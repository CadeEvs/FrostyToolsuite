using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Frosty.Sdk.IO;
using Frosty.Sdk.Managers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Frosty.Sdk.Sdk;

public class TypeSdkGenerator
{
    private long FindTypeInfoOffset(Process process)
    {
        string[] patterns =
        {
            "488b05???????? 48894108 48890d???????? 48???? C3",
            "488b05???????? 48894108 48890d???????? C3",
            "488b05???????? 48894108 48890d????????",
            "488b05???????? 488905???????? 488d05???????? 488905???????? E9",
            "48391D???????? ???? 488b4310",
        };
        
        long startAddress = process.MainModule?.BaseAddress.ToInt64() ?? 0;
        
        using (MemoryReader reader = new(process, startAddress))
        {
            IList<long>? offsets = null;
            foreach (string pattern in patterns)
            {
                reader.Position = startAddress;
                offsets = reader.Scan(pattern);
                if (offsets.Count != 0)
                    break;
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

    public bool CreateSdk()
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
        
        File.WriteAllText("sdk.cs", source);

        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        MetadataReference[] references =
        {
            MetadataReference.CreateFromFile("FrostySdkGenerator.dll", new MetadataReferenceProperties(MetadataImageKind.Assembly)),
            MetadataReference.CreateFromFile("FrostySdk.dll")
        };

        var compilation = CSharpCompilation.Create("EbxTypes", new[] { syntaxTree }, references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true, optimizationLevel: OptimizationLevel.Debug));

        using (FileStream stream = new("sdk.dll", FileMode.Create, FileAccess.Write))
        {
            var result = compilation.Emit(stream);
        }
        
        // TODO: link source generator
        
        return true;
    }
}