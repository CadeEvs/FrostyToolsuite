using System;
using System.Text;
using Frosty.Sdk.Attributes;
using Frosty.Sdk.IO;

namespace Frosty.Sdk.Sdk.TypeInfoDatas;

internal class ArrayInfoData : TypeInfoData
{
    public TypeInfo GetTypeInfo() => TypeInfo.TypeInfoMapping[p_typeInfo];

    private long p_typeInfo;

    public override void Read(MemoryReader reader)
    {
        base.Read(reader);

        if (ProfilesLibrary.HasStrippedTypeNames && string.IsNullOrEmpty(m_name))
        {
            m_name = $"Array_{m_nameHash:x8}";
        }

        p_typeInfo = reader.ReadLong();
    }

    public override void CreateType(StringBuilder sb)
    {
        if (!m_guid.Equals(Guid.Empty))
        {
            sb.AppendLine($"[{nameof(ArrayGuidAttribute)}(\"{m_guid}\")]");
        }
        if (m_nameHash != 0)
        {
            sb.AppendLine($"[{nameof(ArrayHashAttribute)}({(int)m_nameHash})]");
        }
    }
}
