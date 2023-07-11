using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Frosty.Sdk.IO;
using Frosty.Sdk.Managers;
using FrostyTypeSdkGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace Frosty.Sdk.Sdk;

public class TypeSdkGenerator
{
    private long FindTypeInfoOffset(Process process)
    {
        // string[] patterns =
        // {
        //     "488b05???????? 48894108 48890d???????? 48???? C3",
        //     "488b05???????? 48894108 48890d???????? C3",
        //     "488b05???????? 48894108 48890d????????",
        //     "488b05???????? 488905???????? 488d05???????? 488905???????? E9",
        //     "48391D???????? ???? 488b4310", // new games
        // };
        
        long startAddress = process.MainModule?.BaseAddress.ToInt64() ?? 0;
        
        using (MemoryReader reader = new(process, startAddress))
        {
            reader.Position = startAddress;
            IList<long> offsets = reader.Scan(ProfilesLibrary.TypeInfoSignature);

            if (offsets.Count == 0)
            {
                return -1;
            }
            
            reader.Position = offsets![0] + 3;
            int newValue = reader.ReadInt(false);
            reader.Position = offsets[0] + 3 + newValue + 4;
            return reader.ReadLong(false);
        }
    }
    
    public bool DumpTypes(Process process)
    {
        long typeInfoOffset = FindTypeInfoOffset(process);
        if (typeInfoOffset == -1)
        {
            return false;
        }
        using (MemoryReader reader = new(process, typeInfoOffset))
        {
            TypeInfo.TypeInfoMapping.Clear();

            TypeInfo? ti = TypeInfo.ReadTypeInfo(reader);

            do
            {
                ti = ti.GetNextTypeInfo(reader);
            } while (ti != null);
        }

        return TypeInfo.TypeInfoMapping.Count != 0;
    }

    public bool CreateSdk(string filePath)
    {
        StringBuilder sb = new();
        
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using Frosty.Sdk.Attributes;");
        sb.AppendLine("using Frosty.Sdk.Managers;");
        sb.AppendLine("using System.Reflection;");
        sb.AppendLine("using Frosty.Sdk;");
        sb.AppendLine();
        sb.AppendLine("[assembly: SdkVersion(" + FileSystemManager.Head + ")]");
        sb.AppendLine();
        sb.AppendLine("namespace Frosty.Sdk.Ebx;");

        
        foreach (TypeInfo typeInfo in TypeInfo.TypeInfoMapping.Values)
        {
            switch (typeInfo.GetFlags().GetTypeEnum())
            {
                case TypeFlags.TypeEnum.Struct:
                case TypeFlags.TypeEnum.Class:
                case TypeFlags.TypeEnum.Enum:
                case TypeFlags.TypeEnum.Delegate:
                    typeInfo.CreateType(sb);
                    break;

                // primitive types
                case TypeFlags.TypeEnum.String:
                case TypeFlags.TypeEnum.CString:
                case TypeFlags.TypeEnum.FileRef:
                case TypeFlags.TypeEnum.Boolean:
                case TypeFlags.TypeEnum.Int8:
                case TypeFlags.TypeEnum.UInt8:
                case TypeFlags.TypeEnum.Int16:
                case TypeFlags.TypeEnum.UInt16:
                case TypeFlags.TypeEnum.Int32:
                case TypeFlags.TypeEnum.UInt32:
                case TypeFlags.TypeEnum.Int64:
                case TypeFlags.TypeEnum.UInt64:
                case TypeFlags.TypeEnum.Float32:
                case TypeFlags.TypeEnum.Float64:
                case TypeFlags.TypeEnum.Guid:
                case TypeFlags.TypeEnum.Sha1:
                case TypeFlags.TypeEnum.ResourceRef:
                case TypeFlags.TypeEnum.TypeRef:
                case TypeFlags.TypeEnum.BoxedValueRef:
                    typeInfo.CreateType(sb);
                    break;
            }
        }

        string source = sb.ToString();

        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

        List<MetadataReference> references = new();
        
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (Assembly assembly in assemblies)
        {
            if (!assembly.IsDynamic && !string.IsNullOrEmpty(assembly.Location))
            {
                references.Add(MetadataReference.CreateFromFile(assembly.Location));
            }
        }

        
#if EBX_TYPE_SDK_DEBUG
        OptimizationLevel level = OptimizationLevel.Debug;
#else
        OptimizationLevel level = OptimizationLevel.Release;
#endif
        
        CSharpCompilation compilation = CSharpCompilation.Create("EbxTypes", new[] { syntaxTree }, references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true, optimizationLevel: level));
        
        List<AdditionalText> meta = new List<AdditionalText>();
        
        if (Directory.Exists("Meta"))
        {
            foreach (string additionalTextPath in Directory.EnumerateFiles("Meta"))
            {
                meta.Add(new CustomAdditionalText(additionalTextPath));
            }
        }

        GeneratorDriver driver = CSharpGeneratorDriver
            .Create(new SourceGenerator())
            .AddAdditionalTexts(ImmutableArray.CreateRange(meta));
        
        driver.RunGeneratorsAndUpdateCompilation(
            compilation,
            out Compilation outputCompilation,
            out _);

#if EBX_TYPE_SDK_DEBUG
        foreach (SyntaxTree tree in outputCompilation.SyntaxTrees)
        {
            if (string.IsNullOrEmpty(tree.FilePath))
            {
                File.WriteAllText("DumpedTypes.cs", tree.GetText().ToString());
                continue;
            }

            FileInfo fileInfo = new(tree.FilePath);
            Directory.CreateDirectory(fileInfo.DirectoryName!);
            File.WriteAllText(tree.FilePath, tree.GetText().ToString());
        }
#endif
        
        using (FileStream stream = new(filePath, FileMode.Create, FileAccess.Write))
        {
            EmitResult result = outputCompilation.Emit(stream);
            if (!result.Success)
            {
#if EBX_TYPE_SDK_DEBUG
                File.WriteAllLines("Errors.txt", result.Diagnostics.Select(static d => d.ToString()));
#endif
                return false;
            }
        }
        
        return true;
    }
}