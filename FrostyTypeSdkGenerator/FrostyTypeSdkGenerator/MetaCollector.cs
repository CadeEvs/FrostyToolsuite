using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FrostyTypeSdkGenerator;

public class MetaCollector : CSharpSyntaxWalker
{
    public readonly Dictionary<string, Dictionary<string, string>> Meta = new(); 
    // public override void Visit(SyntaxNode? node)
    // {
    //     if (node is not TypeDeclarationSyntax typeDeclarationSyntax)
    //     {
    //         base.Visit(node);
    //         return;
    //     }
    //
    //     string name = typeDeclarationSyntax.Identifier.Text;
    //
    //     // just expect there to be properties
    //     Meta.Add(name, new Dictionary<string, string>());
    //     
    //     foreach (MemberDeclarationSyntax property in typeDeclarationSyntax.Members.Where(static member => member is PropertyDeclarationSyntax))
    //     {
    //         Meta[name].Add(((PropertyDeclarationSyntax)property).Identifier.Text, property.GetText().ToString());
    //     }
    // }

    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        string name = node.Identifier.Text;
        
        // just expect there to be properties
        if (!Meta.ContainsKey(name))
        {
            Meta.Add(name, new Dictionary<string, string>());
        }

        foreach (MemberDeclarationSyntax member in node.Members)
        {
            if (member is PropertyDeclarationSyntax property)
            {
                Meta[name].Add(property.Identifier.Text, property.GetText().ToString());
            }
        }
    }

    public override void VisitStructDeclaration(StructDeclarationSyntax node)
    {
        string name = node.Identifier.Text;
        
        // just expect there to be properties
        if (!Meta.ContainsKey(name))
        {
            Meta.Add(name, new Dictionary<string, string>());
        }

        foreach (MemberDeclarationSyntax member in node.Members)
        {
            if (member is PropertyDeclarationSyntax property)
            {
                Meta[name].Add(property.Identifier.Text, property.GetText().ToString());
            }
        }
    }
}