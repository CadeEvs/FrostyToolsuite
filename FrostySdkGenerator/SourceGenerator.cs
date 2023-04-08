using System.Text;
using Frosty.Sdk.Attributes;
using Frosty.Sdk.Sdk;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace FrostySdkGenerator;

public class FrostySdkGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        if (Directory.Exists("Tests"))
        {
            foreach (string file in Directory.EnumerateFiles("Tests"))
            {
                // TODO: parse files and add classes to a list/dict to pass it into the MainSyntaxReceiver
            }
        }
        context.RegisterForSyntaxNotifications(() => new MainSyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        MainSyntaxReceiver syntaxReceiver = (MainSyntaxReceiver)context.SyntaxReceiver!;

        foreach (BaseClassSyntaxReceiver.Capture capture in syntaxReceiver.ClassSyntaxReceiver.BaseClasses)
        {
            ClassDeclarationSyntax output = capture.ClassDeclarationSyntax.WithMembers(List(CreateInstanceGuid()))
                .NormalizeWhitespace();
            
            context.AddSource($"{capture.ClassDeclarationSyntax.Identifier.Text}.g.cs", output.GetText(Encoding.UTF8));
        }

        foreach (KeyValuePair<StructDeclarationSyntax,List<FieldDeclarationSyntax>> pair in syntaxReceiver.StructSyntaxReceiver.Structs)
        {
            StructDeclarationSyntax output = pair.Key.WithMembers(new SyntaxList<MemberDeclarationSyntax>
            {
                CreateEqualsOverride(pair.Key, pair.Value),
                // TODO: hashcode override
            });
            
            context.AddSource($"{pair.Key.Identifier.Text}.g.cs", output.GetText(Encoding.UTF8));
        }
    }

    private MemberDeclarationSyntax[] CreateInstanceGuid()
    {
        return new MemberDeclarationSyntax[]
        {
            PropertyDeclaration(IdentifierName("AssetClassGuid"), Identifier("__InstanceGuid")).WithAttributeLists(
                    List<AttributeListSyntax>(
                        new AttributeListSyntax[]
                        {
                            AttributeList(
                                SingletonSeparatedList<AttributeSyntax>(
                                    Attribute(
                                        IdentifierName(nameof(IsTransientAttribute))))),
                            AttributeList(
                                SingletonSeparatedList<AttributeSyntax>(
                                    Attribute(
                                        IdentifierName(nameof(IsReadOnlyAttribute))))),
                            AttributeList(
                                SingletonSeparatedList<AttributeSyntax>(
                                    Attribute(
                                            IdentifierName(nameof(DisplayNameAttribute)))
                                        .WithArgumentList(
                                            AttributeArgumentList(
                                                SingletonSeparatedList<AttributeArgumentSyntax>(
                                                    AttributeArgument(
                                                        LiteralExpression(
                                                            SyntaxKind.StringLiteralExpression,
                                                            Literal("Guid")))))))),
                            AttributeList(
                                SingletonSeparatedList<AttributeSyntax>(
                                    Attribute(
                                            IdentifierName(nameof(DisplayNameAttribute)))
                                        .WithArgumentList(
                                            AttributeArgumentList(
                                                SingletonSeparatedList<AttributeArgumentSyntax>(
                                                    AttributeArgument(
                                                        LiteralExpression(
                                                            SyntaxKind.StringLiteralExpression,
                                                            Literal("Annotations")))))))),
                            AttributeList(
                                SingletonSeparatedList<AttributeSyntax>(
                                    Attribute(
                                            IdentifierName(nameof(EbxFieldMetaAttribute)))
                                        .WithArgumentList(
                                            AttributeArgumentList(
                                                SeparatedList<AttributeArgumentSyntax>(
                                            new SyntaxNodeOrToken[]{
                                                AttributeArgument(
                                                    ObjectCreationExpression(
                                                        IdentifierName(nameof(TypeFlags)))
                                                    .WithArgumentList(
                                                        ArgumentList(
                                                            SingletonSeparatedList<ArgumentSyntax>(
                                                                Argument(
                                                                    IdentifierName(nameof(TypeFlags.TypeEnum.Guid))))))),
                                                Token(SyntaxKind.CommaToken),
                                                AttributeArgument(
                                                    LiteralExpression(
                                                        SyntaxKind.NumericLiteralExpression,
                                                        Literal(1))),
                                                Token(SyntaxKind.CommaToken),
                                                AttributeArgument(
                                                    LiteralExpression(
                                                        SyntaxKind.NullLiteralExpression))}))))),
                            AttributeList(
                                SingletonSeparatedList<AttributeSyntax>(
                                    Attribute(
                                            IdentifierName(nameof(FieldIndexAttribute)))
                                        .WithArgumentList(
                                            AttributeArgumentList(
                                                SingletonSeparatedList<AttributeArgumentSyntax>(
                                                    AttributeArgument(
                                                        PrefixUnaryExpression(
                                                            SyntaxKind.UnaryMinusExpression,
                                                            LiteralExpression(
                                                                SyntaxKind.NumericLiteralExpression,
                                                                Literal(1)))))))))
                        }))
                .WithModifiers(
                    TokenList(
                        Token(SyntaxKind.PublicKeyword)))
                .WithAccessorList(
                    AccessorList(
                        SingletonList<AccessorDeclarationSyntax>(
                            AccessorDeclaration(
                                    SyntaxKind.GetAccessorDeclaration)
                                .WithBody(
                                    Block(
                                        SingletonList<StatementSyntax>(
                                            ReturnStatement(
                                                IdentifierName("__Guid")))))))),
            FieldDeclaration(
                    VariableDeclaration(
                            IdentifierName("AssetClassGuid"))
                        .WithVariables(
                            SingletonSeparatedList<VariableDeclaratorSyntax>(
                                VariableDeclarator(
                                    Identifier("__Guid")))))
                .WithModifiers(
                    TokenList(
                        Token(SyntaxKind.ProtectedKeyword))),
            MethodDeclaration(
                    IdentifierName("AssetClassGuid"),
                    Identifier("GetInstanceGuid"))
                .WithModifiers(
                    TokenList(
                        Token(SyntaxKind.PublicKeyword)))
                .WithBody(
                    Block(
                        SingletonList<StatementSyntax>(
                            ReturnStatement(
                                IdentifierName("__Guid"))))),
            MethodDeclaration(
                    PredefinedType(
                        Token(SyntaxKind.VoidKeyword)),
                    Identifier("SetInstanceGuid"))
                .WithModifiers(
                    TokenList(
                        Token(SyntaxKind.PublicKeyword)))
                .WithParameterList(
                    ParameterList(
                        SingletonSeparatedList<ParameterSyntax>(
                            Parameter(
                                    Identifier("newGuid"))
                                .WithType(
                                    IdentifierName("AssetClassGuid")))))
                .WithBody(
                    Block(
                        SingletonList<StatementSyntax>(
                            ExpressionStatement(
                                AssignmentExpression(
                                    SyntaxKind.SimpleAssignmentExpression,
                                    IdentifierName("__Guid"),
                                    IdentifierName("newGuid"))))))
        };
    }

    private MethodDeclarationSyntax CreateEqualsOverride(StructDeclarationSyntax structDeclarationSyntax,
        List<FieldDeclarationSyntax> fieldDeclarationSyntaxes)
    {
        return MethodDeclaration(
                PredefinedType(
                    Token(SyntaxKind.BoolKeyword)),
                Identifier("Equals"))
            .WithModifiers(
                TokenList(
                    new[]
                    {
                        Token(SyntaxKind.PublicKeyword),
                        Token(SyntaxKind.OverrideKeyword)
                    }))
            .WithParameterList(
                ParameterList(
                    SingletonSeparatedList(
                        Parameter(
                                Identifier("obj"))
                            .WithType(
                                NullableType(
                                    PredefinedType(
                                        Token(SyntaxKind.ObjectKeyword)))))))
            .WithBody(
                Block(
                    IfStatement(
                        IsPatternExpression(
                            IdentifierName("obj"),
                            UnaryPattern(
                                DeclarationPattern(
                                    IdentifierName(structDeclarationSyntax.Identifier),
                                    SingleVariableDesignation(
                                        Identifier("b"))))),
                        Block(
                            SingletonList<StatementSyntax>(
                                ReturnStatement(
                                    LiteralExpression(
                                        SyntaxKind.FalseLiteralExpression))))),
                    ReturnStatement(CreateReturn(fieldDeclarationSyntaxes))));
    }

    private ExpressionSyntax CreateReturn(List<FieldDeclarationSyntax> fieldDeclarationSyntaxes)
    {
        if (fieldDeclarationSyntaxes.Count == 1)
        {
            return CreateEqualField(fieldDeclarationSyntaxes[0]);
        }

        FieldDeclarationSyntax first = fieldDeclarationSyntaxes[0];
        fieldDeclarationSyntaxes.RemoveAt(0);
        
        return BinaryExpression(SyntaxKind.LogicalAndExpression, CreateReturn(fieldDeclarationSyntaxes), CreateEqualField(first));
    }
    
    private InvocationExpressionSyntax CreateEqualField(FieldDeclarationSyntax fieldDeclarationSyntax)
    {
        return InvocationExpression(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName(fieldDeclarationSyntax.Declaration.Variables.First().Identifier),
                    IdentifierName("Equals")))
            .WithArgumentList(
                ArgumentList(
                    SingletonSeparatedList<ArgumentSyntax>(
                        Argument(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName("b"),
                                IdentifierName(fieldDeclarationSyntax.Declaration.Variables.First().Identifier))))));
    }
}

public class MainSyntaxReceiver : ISyntaxReceiver
{
    public BaseClassSyntaxReceiver ClassSyntaxReceiver = new();
    public StructSyntaxReceiver StructSyntaxReceiver = new();
    public FieldSyntaxReceiver FieldSyntaxReceiver = new();
    
    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        ClassSyntaxReceiver.OnVisitSyntaxNode(syntaxNode);
        StructSyntaxReceiver.OnVisitSyntaxNode(syntaxNode);
        FieldSyntaxReceiver.OnVisitSyntaxNode(syntaxNode);
    }
}

public class BaseClassSyntaxReceiver : ISyntaxReceiver
{
    public readonly List<Capture> BaseClasses = new();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax)
        {
            if (classDeclarationSyntax.BaseList == null)
            {
                BaseClasses.Add(new Capture(classDeclarationSyntax));
            }
        }
    }

    public record Capture(ClassDeclarationSyntax ClassDeclarationSyntax);
}

public class FieldSyntaxReceiver : ISyntaxReceiver
{
    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is FieldDeclarationSyntax fieldDeclarationSyntax)
        {
            // TODO: create property for field with modification from file and copy over attributes
        }
    }
}

public class StructSyntaxReceiver : ISyntaxReceiver
{
    public readonly Dictionary<StructDeclarationSyntax, List<FieldDeclarationSyntax>> Structs = new();
    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is FieldDeclarationSyntax { Parent: StructDeclarationSyntax structDeclarationSyntax } fieldDeclarationSyntax)
        {
            Structs.TryAdd(structDeclarationSyntax, new List<FieldDeclarationSyntax>());
            Structs[structDeclarationSyntax].Add(fieldDeclarationSyntax);
        }
    }
}

public class Asset
{
    
}

public class TextureAsset : Asset
{
    
}

public partial struct SomeStruct
{
    private int _SomeInt;
    private int _SomeOtherInt;
    private int _AnotherInt;
}

public partial struct SomeStruct
{
    public int SomeInt
    {
        get => _SomeInt;
        set => _SomeInt = value;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not SomeStruct b)
        {
            return false;
        }

        return _SomeInt.Equals(b._SomeInt) &&
               _SomeOtherInt.Equals(b._SomeOtherInt);
    }

    public bool Equals(SomeStruct other)
    {
        return _SomeInt == other._SomeInt && _SomeOtherInt == other._SomeOtherInt && _AnotherInt == other._AnotherInt;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;
            
            hash = (hash * 16777619) ^ _SomeInt.GetHashCode();
            hash = (hash * 16777619) ^ _SomeOtherInt.GetHashCode();
            hash = (hash * 16777619) ^ _AnotherInt.GetHashCode();
            
            return hash;
        }
    }
}