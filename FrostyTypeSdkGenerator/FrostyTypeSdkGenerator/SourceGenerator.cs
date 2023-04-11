using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FrostyTypeSdkGenerator;

[Generator(LanguageNames.CSharp)]
internal sealed partial class SourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Equals and GetHashCode overrides for structs
        {
            IncrementalValuesProvider<TypeContext> syntaxProvider = context.SyntaxProvider.CreateSyntaxProvider(StructPredicate, TypeTransform)
                .Where(static type => type is not null)
                .Select(static (type, _) => TransformType(type!)).WithComparer(TypeContextEqualityComparer.Instance);
        
            context.RegisterSourceOutput(syntaxProvider, CreateStructOverrides);
        }

        // InstanceGuid for base classes
        {
            IncrementalValuesProvider<TypeContext> syntaxProvider = context.SyntaxProvider.CreateSyntaxProvider(BaseClassPredicate, TypeTransform)
                .Where(static type => type is not null)
                .Select(static (type, _) => TransformType(type!)).WithComparer(TypeContextEqualityComparer.Instance);
            
            context.RegisterSourceOutput(syntaxProvider, CreateInstanceGuid);
        }

        // DataContainer classes
        {
            IncrementalValuesProvider<TypeContext> syntaxProvider = context.SyntaxProvider.CreateSyntaxProvider(DataContainerPredicate, TypeTransform)
                .Where(static type => type is not null)
                .Select(static (type, _) => TransformType(type!)).WithComparer(TypeContextEqualityComparer.Instance);
            
            context.RegisterSourceOutput(syntaxProvider, CreateId);
        }
        
        // Create Properties
        {
            Dictionary<string, bool> meta = new();
            // foreach (string file in Directory.EnumerateFiles("Meta"))
            // {
            //     var syntaxTree = CSharpSyntaxTree.ParseText(File.ReadAllText(file));
            //     // TODO: create TransformWithMeta method so CreateProperties can add/modify props from meta
            // }
            
            IncrementalValuesProvider<TypeContext> syntaxProvider = context.SyntaxProvider.CreateSyntaxProvider(TypePredicate, TypeTransform)
                .Where(static type => type is not null)
                .Select(static (type, _) => TransformType(type!)).WithComparer(TypeContextEqualityComparer.Instance);
            
            context.RegisterSourceOutput(syntaxProvider, CreateProperties);
        }
    }

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

    private static INamedTypeSymbol? TypeTransform(GeneratorSyntaxContext syntaxContext,
        CancellationToken cancellationToken)
    {
        if (syntaxContext.Node is not TypeDeclarationSyntax candidate)
        {
            throw new Exception("Not a type");
        }
        
        ISymbol? symbol = ModelExtensions.GetDeclaredSymbol(syntaxContext.SemanticModel, candidate, cancellationToken);

        if (symbol is INamedTypeSymbol typeSymbol)
        {
            return typeSymbol;
        }

        return null;
    }

    private static TypeContext TransformType(INamedTypeSymbol type)
    {
        string? @namespace = type.ContainingNamespace.IsGlobalNamespace
            ? null
            : type.ContainingNamespace.ToDisplayString();

        string name = type.Name;
        bool isValueType = type.IsValueType;
        ImmutableArray<FieldContext> fields = type.GetMembers()
            .Where(static member => member.Kind == SymbolKind.Field)
            .Select(TransformField).ToImmutableArray();

        return new TypeContext(@namespace, name, isValueType, fields);
    }

    private static FieldContext TransformField(ISymbol member)
    {
        if (member is not IFieldSymbol field)
        {
            throw new Exception("Not a field.");
        }

        string name = field.Name;
        string type = field.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        ImmutableArray<string> attributes = field.GetAttributes().Select(static attr => attr.ToString()!).ToImmutableArray();

        return new FieldContext(name, type, attributes);
    }

    private static void CreateStructOverrides(SourceProductionContext context, TypeContext structContext)
    {
        string source = $@"// <auto-generated/>
#nullable enable

{(structContext.Namespace is null ? string.Empty : $"namespace {structContext.Namespace}; \n")}
public partial struct {structContext.Name}
{{
    public bool Equals({structContext.Name} b)
    {{
        return {string.Join(" && ", structContext.Fields.Select(static field => $"{field.Name} == b.{field.Name}"))};
    }}

    public override bool Equals(object? obj)
    {{
        if (obj is not {structContext.Name} b)
        {{
            return false;
        }}

        return Equals(b);
    }}

    public override int GetHashCode()
    {{
        unchecked
        {{
            int hash = (int)2166136261;

            {string.Join("\n            ", structContext.Fields.Select(static field => $"hash = (hash * 16777619) ^ {field.Name}.GetHashCode();"))}

            return hash;
        }}
    }}
}}";
        string qualifiedName = structContext.Namespace is null
            ? structContext.Name
            : $"{structContext.Namespace}.{structContext.Name}";
        context.AddSource($"{qualifiedName}.EqualOverride.g.cs", source);
    }

    private static void CreateInstanceGuid(SourceProductionContext context, TypeContext classContext)
    {
        string source = $@"// <auto-generated/>
#nullable enable
using Frosty.Sdk.Attributes;

{(classContext.Namespace is null ? string.Empty : $"namespace {classContext.Namespace}; \n")}
public partial class {classContext.Name}
{{
    [IsTransientAttribute]
    [IsReadOnlyAttribute]
    [DisplayNameAttribute(""Guid"")]
    [CategoryAttribute(""Annotations"")]
    [EbxFieldMetaAttribute(Frosty.Sdk.Sdk.TypeFlags.TypeEnum.Guid, 1u)]
    [FieldIndexAttribute(-1)]
    public Frosty.Sdk.Ebx.AssetClassGuid __InstanceGuid  => __Guid;

    protected Frosty.Sdk.Ebx.AssetClassGuid __Guid;

    public Frosty.Sdk.Ebx.AssetClassGuid GetInstanceGuid() => __Guid;

    public void SetInstanceGuid(Frosty.Sdk.Ebx.AssetClassGuid newGuid) => __Guid = newGuid;
}}";
        string qualifiedName = classContext.Namespace is null
            ? classContext.Name
            : $"{classContext.Namespace}.{classContext.Name}";
        context.AddSource($"{qualifiedName}.InstanceGuid.g.cs", source);
    }

    private static void CreateId(SourceProductionContext context, TypeContext classContext)
    {
        string source = $@"// <auto-generated/>
#nullable enable
using Frosty.Sdk.Attributes;

{(classContext.Namespace is null ? string.Empty : $"namespace {classContext.Namespace}; \n")}
public partial class {classContext.Name}
{{
    [IsTransientAttribute]
    [IsHiddenAttribute]
    [DisplayNameAttribute(""Id"")]
    [CategoryAttribute(""Annotations"")]
    [EbxFieldMetaAttribute(Frosty.Sdk.Sdk.TypeFlags.TypeEnum.CString, 8u)]
    [FieldIndexAttribute(-2)]
    public Frosty.Sdk.Ebx.CString __Id
    {{
        get => GetId();
        set => __id = value;
    }}
    protected Frosty.Sdk.Ebx.CString __id = new Frosty.Sdk.Ebx.CString();
}}";
        if (classContext.Name != "Asset")
        {
            source = source.Replace($"\n    [IsHiddenAttribute]", string.Empty);
        }

        foreach (FieldContext field in classContext.Fields)
        {
            if (field.Name.Equals("Name", StringComparison.OrdinalIgnoreCase) && field.Type.Equals("global::Frosty.Sdk.Ebx.CString"))
            {
                source = source.Remove(source.Length - 1) + @$"
    protected virtual CString GetId()
    {{
        if (!string.IsNullOrEmpty(__id))
        {{
            return __id;
        }}
        if (!string.IsNullOrEmpty({field.Name})
        {{
            return {field.Name}.Sanitize() 
        }}
        return GetType().Name;
    }}
}}";
            }
        }
        
        string qualifiedName = classContext.Namespace is null
            ? classContext.Name
            : $"{classContext.Namespace}.{classContext.Name}";
        context.AddSource($"{qualifiedName}.Id.g.cs", source);
    }

    private static void CreateProperties(SourceProductionContext context, TypeContext typeContext)
    {
        string source = $@"// <auto-generated/>
#nullable enable

{(typeContext.Namespace is null ? string.Empty : $"namespace {typeContext.Namespace}; \n")}
public partial {(typeContext.IsValueType ? "struct" : "class")} {typeContext.Name}
{{";
        foreach (FieldContext field in typeContext.Fields)
        {
            string prop = $@"
    {string.Join("\n", field.Attributes.Select(static attr => $"[{attr}]"))}
    public {field.Type} {field.Name.Remove(0, 1)}
    {{
        get => {field.Name};
        set => {field.Name} = value;
    }}";
            source += prop;
        }

        source += "\n}";
        string qualifiedName = typeContext.Namespace is null
            ? typeContext.Name
            : $"{typeContext.Namespace}.{typeContext.Name}";
        context.AddSource($"{qualifiedName}.Properties.g.cs", source);
    }
}