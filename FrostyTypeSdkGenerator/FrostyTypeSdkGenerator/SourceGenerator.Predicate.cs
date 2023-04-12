using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FrostyTypeSdkGenerator;

internal sealed partial class SourceGenerator
{
    private static bool StructPredicate(SyntaxNode node, CancellationToken cancellationToken)
    {
        return node is StructDeclarationSyntax { Members.Count: > 0 };
    }

    private static bool BaseClassPredicate(SyntaxNode node, CancellationToken cancellationToken)
    {
        return node is ClassDeclarationSyntax { BaseList: null } classDeclarationSyntax && classDeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword) && !classDeclarationSyntax.Modifiers.Any(SyntaxKind.StaticKeyword);
    }

    private static bool DataContainerPredicate(SyntaxNode node, CancellationToken cancellationToken)
    {
        return node is ClassDeclarationSyntax { BaseList: not null } classDeclarationSyntax && classDeclarationSyntax.BaseList.Types.Any(type => (type.Type as IdentifierNameSyntax)?.Identifier.Text == "DataContainer");
    }

    private static bool TypePredicate(SyntaxNode node, CancellationToken cancellationToken)
    {
        return node is TypeDeclarationSyntax { Members.Count: > 0 } typeDeclarationSyntax && typeDeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword) && !typeDeclarationSyntax.Modifiers.Any(SyntaxKind.StaticKeyword);
    }
}
