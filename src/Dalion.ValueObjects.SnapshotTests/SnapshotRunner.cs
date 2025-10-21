using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Dalion.ValueObjects.SnapshotTests;

public class SnapshotRunner<T>
    where T : IIncrementalGenerator, new()
{
    private const string AssemblyName = "GeneratedValueObjects";
    
    private const LanguageVersion LanguageVersion = Microsoft
        .CodeAnalysis
        .CSharp
        .LanguageVersion
        .CSharp11;
    
    private readonly string _path;

    private string? _source;

    public SnapshotRunner([CallerFilePath] string caller = "")
    {
        var n = caller.LastIndexOf('\\');
        n = n > 0 ? n : caller.LastIndexOf('/');
        _path = Path.Combine(caller.Substring(0, n), "snapshots");
    }

    public SnapshotRunner<T> WithSource(string? source)
    {
        if (source != null && !source.Contains("using System;"))
        {
            source = "using System;\n" + source;
        }
        
        _source = source;
        return this;
    }

    public async Task Run()
    {
        _ = _source ?? throw new InvalidOperationException("No source!");

        VerifySettings? verifySettings = null;

        var (diagnostics, syntaxTrees) = await GetGeneratedOutput();
        Assert.True(
            diagnostics.IsEmpty,
            "The following source code should compile:\n" + _source + "\n"
        );

        var outputFolder = Path.Combine(_path, GetSnapshotDirectoryName());

        await Verify(syntaxTrees.Select(s => s.ToString()), verifySettings)
            .UseDirectory(outputFolder);
    }

    private async Task<(
        ImmutableArray<Diagnostic> Diagnostics,
        SyntaxTree[] GeneratedSource
    )> GetGeneratedOutput()
    {
        var results = await new ProjectBuilder()
            .WithAssemblyName(AssemblyName)
            .WithUserSource(_source!)
            .WithLanguageVersion(LanguageVersion)
            .GetGeneratedOutput<T>();

        return results;
    }

    public static string GetSnapshotDirectoryName()
    {
        return "snap-v9.0";
    }
}
