using System;
using System.Collections.Generic;
using System.Text;
using Frosty.Sdk.Attributes;
using Frosty.Sdk.Ebx;
using Frosty.Sdk.IO;
using Frosty.Sdk.Sdk.TypeInfos;
using IsReadOnlyAttribute = System.Runtime.CompilerServices.IsReadOnlyAttribute;

namespace Frosty.Sdk.Sdk.TypeInfoDatas;

internal class ClassInfoData : TypeInfoData
{
    public ClassInfo GetSuperClassInfo() => (TypeInfo.TypeInfoMapping[p_superClass] as ClassInfo)!;

    private long p_superClass;
    private List<FieldInfo> m_fieldInfos = new();
    private List<MethodInfo> m_methodInfos = new();

    public override void Read(MemoryReader reader)
    {
        base.Read(reader);

        if (!TypeInfo.HasNames && string.IsNullOrEmpty(m_name))
        {
            m_name = $"Class_{m_nameHash:x8}";
        }

        p_superClass = reader.ReadLong();
        if (TypeInfo.Version < 3)
        {
            reader.ReadLong();
        }
        long pFieldInfos = reader.ReadLong();
        long pMethodInfos = reader.ReadLong();

        if (pFieldInfos != 0)
        {
            reader.Position = pFieldInfos;
            for (int i = 0; i < m_fieldCount; i++)
            {
                m_fieldInfos.Add(new FieldInfo());
                m_fieldInfos[i].Read(reader, m_nameHash);
            }
        }
        if (TypeInfo.Version > 5)
        {
            if (pMethodInfos != 0)
            {
                reader.Position = pMethodInfos;
                bool read;
                do
                {
                    var methodInfo = new MethodInfo();
                    read = methodInfo.Read(reader);
                    if (read)
                    {
                        m_methodInfos.Add(methodInfo);
                    }
                } while (read);
            }
        }
    }

    public override void CreateType(StringBuilder sb)
    {
        base.CreateType(sb);

        sb.Append($"public partial class {CleanUpName()}");

        string superClassName = string.Empty;
        int superClassFieldCount = 0;
        ClassInfo superClass = GetSuperClassInfo();
        if (superClass.GetName() != GetName())
        {
            superClassFieldCount = superClass.GetFieldCount();
            sb.Append($" : {superClassName = superClass.GetName()}");
        }
        sb.AppendLine();

        sb.AppendLine("{");

        m_fieldInfos.Sort();
        for (int i = 0; i < m_fieldInfos.Count; i++)
        {
            sb.AppendLine($"[{nameof(FieldIndexAttribute)}({i + superClassFieldCount})]");
            m_fieldInfos[i].CreateField(sb);
            
            // TODO: move to source generator
            // string fieldName = m_fieldInfos[i].GetName();
            // if (fieldName.Equals("Name", StringComparison.OrdinalIgnoreCase) && m_name != "Asset" && m_fieldInfos[i].GetTypeInfo().GetFlags().GetTypeEnum() == TypeFlags.TypeEnum.CString)
            // {
            //     Type tmpType = typeof(EbxClassMetaAttribute);
            //     string namespaceName = tmpType.GetProperties()[4].Name;
            //     tmpType = typeof(GlobalAttributes);
            //     string displayModuleName = tmpType.GetFields()[0].Name;
            //     tmpType = typeof(CString);
            //     string funcName1 = tmpType.GetMethods()[0].Name;
            //     string funcName2 = tmpType.GetMethods()[3].Name;
            //
            //     if (superClassName == "DataContainer")
            //     {
            //         sb.AppendLine("protected virtual CString GetId()\r\n{");
            //         sb.AppendLine("if (__id != \"\") return __id;");
            //         sb.AppendLine("if (_" + fieldName + " != \"\") return _" + fieldName + "." + funcName1 + "();");
            //         sb.AppendLine("if (" + typeof(GlobalAttributes).Name + "." + displayModuleName + ")\r\n{\r\n" + nameof(EbxClassMetaAttribute) + " attr = GetType().GetCustomAttribute<" + nameof(EbxClassMetaAttribute) + ">();\r\nif (attr != null && attr." + namespaceName + " != \"\")\r\nreturn attr." + namespaceName + " + \".\" + GetType().Name;\r\n}\r\nreturn GetType().Name;");
            //         sb.AppendLine("}");
            //         addedGetId = true;
            //     }
            //     else
            //     {
            //         sb.AppendLine("protected override CString GetId()\r\n{");
            //         sb.AppendLine("if (__id != \"\") return __id;");
            //         sb.AppendLine("if (_" + fieldName + " != \"\") return _" + fieldName + "." + funcName1 + "();\r\nreturn base.GetId();");
            //         sb.AppendLine("}");
            //     }
            // }
        }

        // if (superClassName == "DataContainer" && !addedGetId)
        // {
        //     Type tmpType = typeof(EbxClassMetaAttribute);
        //     string namespaceName = tmpType.GetProperties()[4].Name;
        //     tmpType = typeof(GlobalAttributes);
        //     string displayModuleName = tmpType.GetFields()[0].Name;
        //
        //     sb.AppendLine("protected virtual CString GetId()\r\n{");
        //     sb.AppendLine("if (__id == \"\")\r\n{\r\nif (" + typeof(GlobalAttributes).Name + "." + displayModuleName + ")\r\n{\r\n" + nameof(EbxClassMetaAttribute) + " attr = GetType().GetCustomAttribute<" + nameof(EbxClassMetaAttribute) + ">();\r\nif (attr != null && attr." + namespaceName + " != \"\")\r\nreturn attr." + namespaceName + " + \".\" + GetType().Name;\r\n}\r\nreturn GetType().Name;\r\n}\r\nreturn __id;");
        //     sb.AppendLine("}");
        // }

        // TODO: what to do with functions
        foreach (MethodInfo method in m_methodInfos)
        {
            method.GetFunctionInfo()?.CreateType(sb);
        }

        sb.AppendLine("}");
    }

    public int GetFieldCount() => m_fieldCount;
}