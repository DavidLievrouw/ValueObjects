using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.IO.Compression;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Dalion.ValueObjects.SnapshotTests;

public sealed class ProjectBuilder
{
    private static readonly ConcurrentDictionary<string, Lazy<Task<string[]>>> NuGetCache = new(
        StringComparer.Ordinal
    );

    private static readonly KeyValuePair<string, ReportDiagnostic>[] SuppressedDiagnostics =
    {
        new("CS8019", ReportDiagnostic.Suppress), // Unnecessary using directive
        new("CS1701", ReportDiagnostic.Suppress), //  Assuming assembly reference
    };

    private readonly IList<MetadataReference> _references = new List<MetadataReference>();
    private string _assemblyName = "generator";
    private LanguageVersion _languageVersion = LanguageVersion.Default;
    private string _userSource = string.Empty;

    public ProjectBuilder WithLanguageVersion(LanguageVersion languageVersion)
    {
        _languageVersion = languageVersion;
        return this;
    }

    public void AddNuGetReference(string packageName, string version, string pathPrefix)
    {
        foreach (var reference in GetNuGetReferences(packageName, version, pathPrefix).Result)
        {
            _references.Add(MetadataReference.CreateFromFile(reference));
        }
    }

    private void AddNuGetReferences()
    {
        AddNuGetReference("Microsoft.NETCore.App.Ref", "9.0.0", "ref/net9.0/");
        AddNuGetReference("FluentValidation", "12.0.0", "lib/net8.0/");
    }

    private static Task<string[]> GetNuGetReferences(
        string packageName,
        string version,
        string path
    )
    {
        var task = NuGetCache.GetOrAdd(
            $"{packageName}@{version}:{path}",
            _ => new Lazy<Task<string[]>>(Download)
        );

        return task.Value;

        async Task<string[]> Download()
        {
            var tempFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Dalion.ValueObjects.SnapshotTests",
                "ref",
                packageName + '@' + version
            );

            if (
                !Directory.Exists(tempFolder)
                || !Directory.EnumerateFileSystemEntries(tempFolder).Any()
            )
            {
                Directory.CreateDirectory(tempFolder);
                using var httpClient = new HttpClient();
                await using var stream = await httpClient
                    .GetStreamAsync(
                        new Uri($"https://www.nuget.org/api/v2/package/{packageName}/{version}")
                    )
                    .ConfigureAwait(false);
                using var zip = new ZipArchive(stream, ZipArchiveMode.Read);

                foreach (
                    var entry in zip.Entries.Where(file =>
                        file.FullName.StartsWith(path, StringComparison.Ordinal)
                    )
                )
                {
                    entry.ExtractToFile(Path.Combine(tempFolder, entry.Name), true);
                }
            }

            var nameAndPathsForDlls = Directory.GetFiles(tempFolder, "*.dll");

            var result = new List<string>();
            foreach (var eachDllNameAndPath in nameAndPathsForDlls)
            {
                try
                {
                    await using var stream = File.OpenRead(eachDllNameAndPath);
                    using var peFile = new PEReader(stream);
                    // ReSharper disable once UnusedVariable
                    var metadataReader = peFile.GetMetadataReader();
                    result.Add(eachDllNameAndPath);
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch { }
            }

            if (result.Count == 0)
            {
                throw new InvalidOperationException(
                    $"Did not add any DLLs as references for {packageName}, v {version}, at {path}!"
                );
            }

            return result.ToArray();
        }
    }

    public ProjectBuilder WithUserSource(string userSource)
    {
        _userSource = userSource;

        return this;
    }

    public ProjectBuilder WithAssemblyName(string assemblyName)
    {
        _assemblyName = assemblyName;

        return this;
    }

    public Task<(
        ImmutableArray<Diagnostic> Diagnostics,
        SyntaxTree[] GeneratedSource
    )> GetGeneratedOutput<T>()
        where T : IIncrementalGenerator, new()
    {
        var parseOptions = new CSharpParseOptions(_languageVersion);

        var usersSyntaxTree = CSharpSyntaxTree.ParseText(_userSource, parseOptions);

        var valueObjectsMetadata = MetadataReference.CreateFromFile(
            typeof(ValueObjectAttribute).Assembly.Location
        );
        _references.Add(valueObjectsMetadata);

        AddNuGetReferences();

        var options = new CSharpCompilationOptions(
            OutputKind.DynamicallyLinkedLibrary,
            moduleName: "ValueObjectsTests",
            specificDiagnosticOptions: SuppressedDiagnostics
        ).WithNullableContextOptions(NullableContextOptions.Enable);

        var compilation = CSharpCompilation.Create(
            _assemblyName,
            [usersSyntaxTree],
            _references,
            options
        );

        var valueObjectGenerator = new T();
        var regexGenerator = GetRegexGenerator();

        var driver = CSharpGeneratorDriver
            .Create(valueObjectGenerator, regexGenerator)
            .WithUpdatedParseOptions(
                parseOptions.WithDocumentationMode(DocumentationMode.Diagnose)
            );

        driver.RunGeneratorsAndUpdateCompilation(
            compilation,
            out var outputCompilation,
            out var generatorDiags
        );

        var finalDiags = outputCompilation.GetDiagnostics();

        if (generatorDiags.Length != 0)
        {
            return Task.FromResult<(
                ImmutableArray<Diagnostic> Diagnostics,
                SyntaxTree[] GeneratedSource
            )>((generatorDiags, []));
        }

        if (finalDiags.Length != 0)
        {
            return Task.FromResult<(
                ImmutableArray<Diagnostic> Diagnostics,
                SyntaxTree[] GeneratedSource
            )>((finalDiags, []));
        }

        return Task.FromResult(
            (
                generatorDiags,
                outputCompilation.SyntaxTrees.Except(compilation.SyntaxTrees).ToArray()
            )
        );
    }

    private static IIncrementalGenerator GetRegexGenerator()
    {
        var dotnetRoot = FindDotNetRoot();
        var packsDir = Path.Combine(dotnetRoot, "packs", "Microsoft.NETCore.App.Ref");

        if (!Directory.Exists(packsDir))
        {
            throw new DirectoryNotFoundException($"Could not find packs directory: {packsDir}");
        }

        var versionDirs = Directory
            .GetDirectories(packsDir)
            .Select(Path.GetFileName)
            .Where(v => Version.TryParse(v, out _))
            .Select(v => new Version(v!))
            .OrderByDescending(v => v)
            .ToList();

        if (versionDirs.Count == 0)
        {
            throw new InvalidOperationException("No .NETCore.App.Ref versions found.");
        }

        var latestVersion = versionDirs.First().ToString();
        var generatorPath = Path.Combine(
            packsDir,
            latestVersion,
            "analyzers",
            "dotnet",
            "cs",
            "System.Text.RegularExpressions.Generator.dll"
        );

        if (!File.Exists(generatorPath))
        {
            throw new FileNotFoundException($"Could not find RegexGenerator at: {generatorPath}");
        }

        var regexGenAssembly = Assembly.LoadFrom(generatorPath);

        var generatorType = regexGenAssembly
            .GetTypes()
            .Where(t =>
                t.GetInterfaces().Any(i => i.FullName == typeof(IIncrementalGenerator).FullName)
            )
            .Single(t => t.Name == "RegexGenerator");

        var regexGenerator = (IIncrementalGenerator)Activator.CreateInstance(generatorType)!;

        return regexGenerator;
    }

    private static string FindDotNetRoot()
    {
        var dotnetRoot = Environment.GetEnvironmentVariable("DOTNET_ROOT");
        if (
            !string.IsNullOrEmpty(dotnetRoot) && Directory.Exists(Path.Combine(dotnetRoot, "packs"))
        )
        {
            return dotnetRoot;
        }

        var dir = new DirectoryInfo(Path.GetDirectoryName(typeof(object).Assembly.Location)!);
        while (dir != null)
        {
            var packsPath = Path.Combine(dir.FullName, "packs");
            if (Directory.Exists(packsPath))
            {
                return dir.FullName;
            }

            dir = dir.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not locate the .NET SDK root directory containing 'packs'."
        );
    }
}
