using System.Collections.Generic;
using System.Text;
using Frosty.Sdk.Attributes;
using Frosty.Sdk.IO;

namespace Frosty.Sdk.Sdk.TypeInfoDatas;

internal class StructInfoData : TypeInfoData
{
    private List<FieldInfo> m_fieldInfos = new List<FieldInfo>();

    public override void Read(MemoryReader reader)
    {
        base.Read(reader);

        if (!TypeInfo.HasNames && string.IsNullOrEmpty(m_name))
        {
            m_name = $"Struct_{m_nameHash:x8}";
        }

        if (TypeInfo.Version > 2)
        {
            reader.ReadLong();
            reader.ReadLong();
            if (TypeInfo.Version > 3)
            {
                reader.ReadLong();
                reader.ReadLong();
                if (TypeInfo.Version > 4)
                {
                    reader.ReadLong();
                }
            }
        }

        long pDefaultValue = reader.ReadLong();
        long pFieldInfos = reader.ReadLong();

        reader.Position = pFieldInfos;
        for (int i = 0; i < m_fieldCount; i++)
        {
            m_fieldInfos.Add(new FieldInfo());
            m_fieldInfos[i].Read(reader, m_nameHash);
        }
    }

    public override void CreateType(StringBuilder sb)
    {
        base.CreateType(sb);

        sb.AppendLine($"public partial struct {CleanUpName()}");

        sb.AppendLine("{");

        m_fieldInfos.Sort();
        for (int i = 0; i < m_fieldInfos.Count; i++)
        {
            sb.AppendLine($"[{nameof(FieldIndexAttribute)}({i})]");
            m_fieldInfos[i].CreateField(sb);
        }

        // move to source generator
        // if (m_fieldInfos.Count > 0)
        // {
        //     // Equals override
        //     sb.AppendLine("public override bool Equals(object obj)\r\n{");
        //     sb.AppendLine("if (obj == null || !(obj is " + CleanUpName() + "))\r\nreturn false;");
        //     sb.AppendLine(CleanUpName() + " b = (" + CleanUpName() + ")obj;");
        //     sb.Append("return ");
        //
        //     for (int i = 0; i < m_fieldInfos.Count; i++)
        //     {
        //         string fieldName = m_fieldInfos[i].GetName();
        //         sb.AppendLine(((i != 0) ? "&& " : "") + fieldName + ".Equals(b." + fieldName + ")");
        //     }
        //     sb.AppendLine(";\r\n}");
        //
        //     // GetHashCode override
        //     sb.AppendLine("public override int GetHashCode()\r\n{\r\nunchecked {\r\nint hash = (int)2166136261;");
        //     for (int i = 0; i < m_fieldInfos.Count; i++)
        //     {
        //         string fieldName = m_fieldInfos[i].GetName();
        //         sb.AppendLine("hash = (hash * 16777619) ^ " + fieldName + ".GetHashCode();");
        //     }
        //     sb.AppendLine("return hash;\r\n}\r\n}");
        // }

        sb.AppendLine("}");
    }
}