using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Dalion.ValueObjects.Generation;

internal static partial class Extensions
{
    public static SourceText WithNamespace(this CompilationUnitSyntax root, string newNs)
    {
        // Remove existing file-scoped namespace declaration (if any)
        var members = root.Members;
        var fileScopedNamespace = members
            .OfType<FileScopedNamespaceDeclarationSyntax>()
            .FirstOrDefault();

        if (fileScopedNamespace != null)
        {
            members = fileScopedNamespace.Members;
        }

        // Create a new namespace declaration with the specified namespace name
        var newNamespace = SyntaxFactory
            .NamespaceDeclaration(SyntaxFactory.ParseName(newNs))
            .WithMembers(members)
            .NormalizeWhitespace();

        // Create a new compilation unit preserving existing usings but replacing members with the new namespace
        var newCompilationUnit = SyntaxFactory
            .CompilationUnit()
            .WithUsings(root.Usings)
            .WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(newNamespace))
            .NormalizeWhitespace();

        // Convert to source text
        var sourceCode = newCompilationUnit.ToFullString();
        return SourceText.From(sourceCode, Encoding.UTF8);
    }
}
