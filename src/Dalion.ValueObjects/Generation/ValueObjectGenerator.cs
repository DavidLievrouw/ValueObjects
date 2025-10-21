using System.Collections.Immutable;
using System.Linq;
using Dalion.ValueObjects.Generation.Fragments;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Dalion.ValueObjects.Generation;

/// <inheritdoc />
[Generator]
public class ValueObjectGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        EnsureValueObjectAttribute(context);

        var valueObjectsProvider = context.SyntaxProvider.CreateSyntaxProvider(
            (node, _) => node is TypeDeclarationSyntax { AttributeLists.Count: > 0 },
            (ctx, _) =>
            {
                var syntaxInformation = (TypeDeclarationSyntax)ctx.Node;

                var semanticModel = ctx.SemanticModel;

                var declaredSymbol = semanticModel.GetDeclaredSymbol(ctx.Node)!;

                var symbolInformation = (INamedTypeSymbol)declaredSymbol;

                var attributeData = symbolInformation
                    .TryGetValueObjectAttributes()
                    .ToImmutableArray();

                if (attributeData.Length > 0)
                {
                    return new GenerationTarget(
                        semanticModel,
                        syntaxInformation,
                        symbolInformation,
                        attributeData.First()
                    );
                }

                return null;
            }
        );

        context.RegisterSourceOutput(
            valueObjectsProvider,
            (sourceProductionContext, target) =>
            {
                if (target is null)
                {
                    return;
                }

                AddGeneratedValueObject(target, sourceProductionContext);
                AddGeneratedFluentValidationExtensions(target, sourceProductionContext);
                AddGeneratedUnderlyingTypeCreationExtensions(target, sourceProductionContext);
            }
        );
    }

    private static void AddGeneratedFluentValidationExtensions(
        GenerationTarget target,
        SourceProductionContext context
    )
    {
        var config = target.GetAttributeConfiguration();

        var code = new FluentValidationExtensionsProvider().ProvideFragment(config, target);

        if (string.IsNullOrEmpty(code))
        {
            return;
        }

        context.AddSource($"{config.TypeName}FluentValidationExtensions.g.cs", code!);
    }

    private void AddGeneratedUnderlyingTypeCreationExtensions(
        GenerationTarget target,
        SourceProductionContext context
    )
    {
        var config = target.GetAttributeConfiguration();

        var code = new UnderlyingTypeCreationExtensionsProvider().ProvideFragment(config, target);

        if (string.IsNullOrEmpty(code))
        {
            return;
        }

        context.AddSource($"{config.TypeName}UnderlyingTypeCreationExtensions.g.cs", code!);
    }

    private void AddGeneratedValueObject(GenerationTarget target, SourceProductionContext context)
    {
        var config = target.GetAttributeConfiguration();

        var creation = new CreationProvider().ProvideFragment(config, target);

        var equality = new EqualityProvider().ProvideFragment(config, target);
        var equalityUnderlyingType = new EqualityUnderlyingTypeProvider().ProvideFragment(
            config,
            target
        );
        var equalityOperators = new EqualityOperatorsUnderlyingTypeProvider().ProvideFragment(
            config,
            target
        );

        var comparison = new ComparisonProvider().ProvideFragment(config, target);

        var conversion = new ConversionProvider().ProvideFragment(config, target);

        var validationClass = new ValidationClassProvider().ProvideFragment(config, target);

        var validationMembers = new ValidationMembersProvider().ProvideFragment(config, target);

        var toStringOverrides = new ToStringProvider().ProvideFragment(config, target);

        var interfaceImplementations = new InterfaceImplProvider().ProvideFragment(config, target);

        var jsonConverter = new JsonConverterFragmentProvider().ProvideFragment(config, target);
        var typeConverter = new TypeConverterFragmentProvider().ProvideFragment(config, target);

        var preSetValueCache = new PreSetValueCacheProvider().ProvideFragment(config, target);

        var parsable = new ParsableProvider().ProvideFragment(config, target);

        var containingTypes = target.SymbolInformation.GetContainingTypesDeclarations();
        var closingBraces = target.SymbolInformation.GetClosingBraces();
        var defaultValue = new DefaultValueProvider().ProvideFragment(config, target);

        var generatedType =
            $@"
#nullable enable

using System;

namespace {config.Namespace} {{
    {containingTypes}
    [System.Diagnostics.DebuggerDisplay(""{config.TypeName} {{Value}}"")]
    [System.Text.Json.Serialization.JsonConverter(typeof({config.TypeName}SystemTextJsonConverter))]
    [System.ComponentModel.TypeConverter(typeof({config.TypeName}TypeConverter))]
    public partial record struct {config.TypeName} {interfaceImplementations} {{
        private readonly {config.UnderlyingTypeName} _value;
        private readonly bool _initialized;
#pragma warning disable CS0414
        private readonly bool _isNullOrEmpty;
#pragma warning restore CS0414
        private readonly Validation _validation;
        private static readonly Type UnderlyingType = typeof({config.UnderlyingTypeName});

        /// <summary>
        ///     Gets the underlying value of this <see cref=""{config.TypeName}""/>.
        /// </summary>
        public {config.UnderlyingTypeName} Value => _value;

        {creation}

        /// <summary>
        ///     Represents a <see cref=""{config.TypeName}""/> with a default underlying value.
        /// </summary>
        public static {config.TypeName} {config.EmptyValueName} {{ get; }} = new {config.TypeName}({defaultValue});

        /// <summary>
        ///     Indicates whether this <see cref=""{config.TypeName}""/> has been
        ///     initialized with a value.
        /// </summary>
        /// <returns><see langword=""true"" /> if this <see cref=""{config.TypeName}""/> has been initialized; otherwise, <see langword=""false"" />.</returns>
        public bool IsInitialized() => _initialized;

        {equality}

        {equalityUnderlyingType}

        {equalityOperators}

        {comparison}

        {conversion}

        {toStringOverrides}

        {validationMembers}

        {validationClass}

        {parsable}

        {jsonConverter}

        {typeConverter}

        {preSetValueCache}
    }}
    {closingBraces}
}}";

        context.AddSource($"{config.TypeName}.g.cs", generatedType);
    }

    private void EnsureValueObjectAttribute(IncrementalGeneratorInitializationContext context)
    {
        var namespaceName = context.AnalyzerConfigOptionsProvider.Select(
            (p, _) =>
                p.GlobalOptions.TryGetValue("build_property.RootNamespace", out var ns)
                && !string.IsNullOrWhiteSpace(ns)
                    ? ns
                : p.GlobalOptions.TryGetValue("build_property.AssemblyName", out var asm)
                && !string.IsNullOrWhiteSpace(asm)
                    ? asm
                : typeof(ValueObjectAttribute).Namespace
        );

        context.RegisterSourceOutput(
            namespaceName,
            (spc, ns) =>
            {
                var attributeContent =
                    typeof(ValueObjectAttribute).Assembly.GetEmbeddedResourceString(
                        $"{nameof(ValueObjectAttribute)}.cs"
                    );

                if (attributeContent == null)
                {
                    return;
                }

                var tree = CSharpSyntaxTree.ParseText(attributeContent);
                var root = (CompilationUnitSyntax)tree.GetRoot();
                var fixedSource = root.WithNamespace(ns ?? "global::");

                spc.AddSource($"{nameof(ValueObjectAttribute)}.g.cs", fixedSource);
            }
        );
    }
}
