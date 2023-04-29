using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FrostyTypeSdkGenerator;

[Generator(LanguageNames.CSharp)]
public sealed partial class SourceGenerator : IIncrementalGenerator
{
    private static readonly MetaCollector s_metaCollector = new();
    
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Equals and GetHashCode overrides for structs
        {
            IncrementalValuesProvider<TypeContext> syntaxProvider = context.SyntaxProvider.CreateSyntaxProvider(StructPredicate, TypeTransform)
                .Where(static type => type is not null)
                .Select(static (type, _) => TransformType(type!)).WithComparer(TypeContextEqualityComparer.Instance);
        
            context.RegisterSourceOutput(syntaxProvider, CreateStructOverrides);
        }
        
        // Equals override for empty structs
        {
            IncrementalValuesProvider<TypeContext> syntaxProvider = context.SyntaxProvider.CreateSyntaxProvider(EmptyStructPredicate, TypeTransform)
                .Where(static type => type is not null)
                .Select(static (type, _) => TransformType(type!)).WithComparer(TypeContextEqualityComparer.Instance);
        
            context.RegisterSourceOutput(syntaxProvider, CreateEmptyStructOverrides);
        }

        // InstanceGuid for base classes
        {
            IncrementalValuesProvider<TypeContext> syntaxProvider = context.SyntaxProvider.CreateSyntaxProvider(BaseClassPredicate, TypeTransform)
                .Where(static type => type is not null)
                .Select(static (type, _) => TransformType(type!)).WithComparer(TypeContextEqualityComparer.Instance);
            
            context.RegisterSourceOutput(syntaxProvider, CreateInstanceGuid);
        }

        // Id for DataContainer classes
        {
            IncrementalValuesProvider<TypeContext> syntaxProvider = context.SyntaxProvider.CreateSyntaxProvider(DataContainerPredicate, TypeTransform)
                .Where(static type => type is not null)
                .Select(static (type, _) => TransformType(type!)).WithComparer(TypeContextEqualityComparer.Instance);
            
            context.RegisterSourceOutput(syntaxProvider, CreateId);
        }
        
        // Id for non DataContainer classes
        {
            IncrementalValuesProvider<TypeContext> syntaxProvider = context.SyntaxProvider.CreateSyntaxProvider(NonDataContainerPredicate, TypeTransform)
                .Where(static type => type is not null)
                .Select(static (type, _) => TransformType(type!)).WithComparer(TypeContextEqualityComparer.Instance);
            
            context.RegisterSourceOutput(syntaxProvider, CreateIdOverride);
        }
        
        // Create Properties
        {
            s_metaCollector.Meta.Clear();
            IncrementalValuesProvider<string> metaProvider = context.AdditionalTextsProvider.Where(static meta => meta.Path.EndsWith(".cs"))
                .Select((meta, _) => meta.GetText(_)!.ToString());
            context.RegisterSourceOutput(metaProvider, CreateMeta);
            
            IncrementalValuesProvider<TypeContext> syntaxProvider = context.SyntaxProvider.CreateSyntaxProvider(TypePredicate, TypeTransform)
                .Where(static type => type is not null)
                .Select(static (type, _) => TransformType(type!)).WithComparer(TypeContextEqualityComparer.Instance);
            
            context.RegisterSourceOutput(syntaxProvider, CreateProperties);
        }
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
using System.Linq;

{(structContext.Namespace is null ? string.Empty : $"namespace {structContext.Namespace}; \n")}
public partial struct {structContext.Name}
{{
    public bool Equals({structContext.Name} b)
    {{
        return {string.Join(" && ", structContext.Fields.Select(static field => field.Type.Contains("global::System.Collections.Generic.List<") ? $"{field.Name}.SequenceEqual(b.{field.Name})" : $"{field.Name} == b.{field.Name}"))};
    }}

    public override bool Equals(object? obj)
    {{
        if (obj is not {structContext.Name} b)
        {{
            return false;
        }}

        return Equals(b);
    }}

    public static bool operator ==({structContext.Name} a, object b) => a.Equals(b);

    public static bool operator !=({structContext.Name} a, object b) => !a.Equals(b);

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
        
        context.AddSource($"{GetQualifiedName(structContext)}.EqualOverride.g.cs", source);
    }

    private static void CreateEmptyStructOverrides(SourceProductionContext context, TypeContext structContext)
    {
        string source = $@"// <auto-generated/>
#nullable enable

{(structContext.Namespace is null ? string.Empty : $"namespace {structContext.Namespace}; \n")}
public partial struct {structContext.Name}
{{
    public bool Equals({structContext.Name} b)
    {{
        return true;
    }}

    public override bool Equals(object? obj)
    {{
        if (obj is not {structContext.Name} b)
        {{
            return false;
        }}

        return Equals(b);
    }}

    public static bool operator ==({structContext.Name} a, object b) => a.Equals(b);

    public static bool operator !=({structContext.Name} a, object b) => !a.Equals(b);

    public override int GetHashCode()
    {{
        unchecked
        {{
            return (int)2166136261;
        }}
    }}
}}";
        
        context.AddSource($"{GetQualifiedName(structContext)}.EmptyEqualOverride.g.cs", source);
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
    public global::Frosty.Sdk.Ebx.AssetClassGuid __InstanceGuid  => __Guid;

    protected global::Frosty.Sdk.Ebx.AssetClassGuid __Guid;

    public global::Frosty.Sdk.Ebx.AssetClassGuid GetInstanceGuid() => __Guid;

    public void SetInstanceGuid(global::Frosty.Sdk.Ebx.AssetClassGuid newGuid) => __Guid = newGuid;
}}";
        
        context.AddSource($"{GetQualifiedName(classContext)}.InstanceGuid.g.cs", source);
    }

    private static void CreateId(SourceProductionContext context, TypeContext classContext)
    {
        string source = $@"// <auto-generated/>
#nullable enable
using Frosty.Sdk.Attributes;
using System.Reflection;

{(classContext.Namespace is null ? string.Empty : $"namespace {classContext.Namespace}; \n")}
public partial class {classContext.Name}
{{
    [IsTransientAttribute]
    [IsHiddenAttribute]
    [DisplayNameAttribute(""Id"")]
    [CategoryAttribute(""Annotations"")]
    [EbxFieldMetaAttribute(Frosty.Sdk.Sdk.TypeFlags.TypeEnum.CString, 8u)]
    [FieldIndexAttribute(-2)]
    public global::Frosty.Sdk.Ebx.CString __Id
    {{
        get => GetId();
        set => __id = value;
    }}
    protected global::Frosty.Sdk.Ebx.CString __id = new global::Frosty.Sdk.Ebx.CString();
}}";
        if (classContext.Name != "Asset")
        {
            source = source.Replace($"\n    [IsHiddenAttribute]", string.Empty);
        }

        bool added = false;
        foreach (FieldContext field in classContext.Fields)
        {
            if (classContext.Name != "Asset" && field.Name.Equals("_Name", StringComparison.OrdinalIgnoreCase) && field.Type.Equals("global::Frosty.Sdk.Ebx.CString"))
            {
                source = source.Remove(source.Length - 1) + @$"
    protected virtual global::Frosty.Sdk.Ebx.CString GetId()
    {{
        if (!string.IsNullOrEmpty(__id))
        {{
            return __id;
        }}
        if (!string.IsNullOrEmpty({field.Name}))
        {{
            return {field.Name}.Sanitize();
        }}

        if (GlobalAttributes.DisplayModuleInClassId)
        {{
            EbxTypeMetaAttribute? customAttribute = GetType().GetCustomAttribute<EbxTypeMetaAttribute>();
			if (customAttribute is not null && !string.IsNullOrEmpty(customAttribute.Namespace))
			{{
                return $""{{customAttribute.Namespace}}.{{GetType().Name}}"";
            }}
        }}

        return GetType().Name;
    }}
}}";
                added = true;
                break;
            }
        }

        if (!added)
        {
            source = source.Remove(source.Length - 1) + @$"
    protected virtual global::Frosty.Sdk.Ebx.CString GetId()
    {{
        if (!string.IsNullOrEmpty(__id))
        {{
            return __id;
        }}

        if (GlobalAttributes.DisplayModuleInClassId)
        {{
            EbxTypeMetaAttribute? customAttribute = GetType().GetCustomAttribute<EbxTypeMetaAttribute>();
			if (customAttribute is not null && !string.IsNullOrEmpty(customAttribute.Namespace))
			{{
                return $""{{customAttribute.Namespace}}.{{GetType().Name}}"";
            }}
        }}

        return GetType().Name;
    }}
}}";
        }
        
        context.AddSource($"{GetQualifiedName(classContext)}.Id.g.cs", source);
    }
    
    private static void CreateIdOverride(SourceProductionContext context, TypeContext classContext)
    {
        FieldContext field = classContext.Fields.First(f => f.Name == "_Name");

        if (!field.Type.Equals("global::Frosty.Sdk.Ebx.CString"))
        {
            return;
        }
        
        string source = $@"// <auto-generated/>
#nullable enable
using Frosty.Sdk.Attributes;
using System.Reflection;

{(classContext.Namespace is null ? string.Empty : $"namespace {classContext.Namespace}; \n")}
public partial class {classContext.Name}
{{
    protected override CString GetId()
    {{
        if (!string.IsNullOrEmpty(__id))
        {{
            return __id;
        }}
        
        if (!string.IsNullOrEmpty({field.Name}))
        {{
            return {field.Name}.Sanitize();
        }}

        return base.GetId();
    }}
}}";
        
        context.AddSource($"{GetQualifiedName(classContext)}.OverrideId.g.cs", source);
    }

    private static void CreateProperties(SourceProductionContext context, TypeContext typeContext)
    {
        s_metaCollector.Meta.TryGetValue(typeContext.Name, out Dictionary<string, string>? meta);
        
        string source = $@"// <auto-generated/>
#nullable enable

{(typeContext.Namespace is null ? string.Empty : $"namespace {typeContext.Namespace}; \n")}
public partial {(typeContext.IsValueType ? "struct" : "class")} {typeContext.Name}
{{";
        bool needsConstructor = typeContext.Fields.Any(static f => f.Type.Contains("global::System.Collections.Generic.List<"));
        string constructor = string.Empty;
        foreach (FieldContext field in typeContext.Fields)
        {
            if (needsConstructor)
            {
                if (field.Type.Contains("global::System.Collections.Generic.List<"))
                {
                    constructor += $"\n        {field.Name} = new();";
                }
                else
                {
                    constructor += $"\n        {field.Name} = default;";
                }
            }
            
            string prop;
            if (meta is not null && meta.TryGetValue(field.Name.Remove(0, 1), out string? metaProp))
            {
                meta.Remove(field.Name.Remove(0, 1));
                prop = @$"
{string.Join("\n", field.Attributes.Select(static attr => $"    [{attr}]"))}
{metaProp}";
            }
            else
            {
                prop = $@"
{string.Join("\n", field.Attributes.Select(static attr => $"    [{attr}]"))}
    public {field.Type} {field.Name.Remove(0, 1)}
    {{
        get => {field.Name};
        set => {field.Name} = value;
    }}";
            }
            
            source += prop;
        }

        if (meta is not null)
        {
            foreach (string addedProps in meta.Values)
            {
                source += @$"

{addedProps}";
            }
        }

        if (needsConstructor)
        {
            
            source += $@"

    public {typeContext.Name}()
    {{{constructor}
    }}";
        }
        
        source += "\n}";
        
        context.AddSource($"{GetQualifiedName(typeContext)}.Properties.g.cs", source);
    }

    private static void CreateMeta(SourceProductionContext context, string meta)
    {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(meta);
        CompilationUnitSyntax root = syntaxTree.GetCompilationUnitRoot();
        s_metaCollector.Visit(root);
    }

    private static string GetQualifiedName(TypeContext context)
    {
        string hash = QuickHash(context.Name).ToString("x8");
        return context.Namespace is null
            ? $"{context.Name}.{hash}"
            : $"{context.Namespace}.{context.Name}.{hash}";
    }
    
    private static uint QuickHash(string value)
    {
        const uint kOffset = 5381;
        const uint kPrime = 33;
        
        uint hash = kOffset;
        for (int i = 0; i < value.Length; i++)
        {
            hash = (hash * kPrime) ^ value[i];
        }

        return (uint)hash;
    }
}