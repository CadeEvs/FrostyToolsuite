using Frosty.Sdk.IO;
using Frosty.Sdk.Sdk.TypeInfoDatas;

namespace Frosty.Sdk.Sdk.TypeInfos;

internal class ClassInfo : TypeInfo
{
    public ClassInfo GetSuperClassInfo() => (TypeInfo.TypeInfoMapping[p_superClass] as ClassInfo)!;

    private long p_superClass;
    private long p_defaultInstance;

    public ClassInfo(ClassInfoData data)
        : base(data)
    {
    }

    public override void Read(MemoryReader reader)
    {
        base.Read(reader);

        p_superClass = reader.ReadLong();
        p_defaultInstance = reader.ReadLong();
    }

    public int GetFieldCount()
    {
        int fieldCount = (m_data as ClassInfoData)?.GetFieldCount() ?? 0;
        ClassInfo superClass = GetSuperClassInfo();
        if (superClass != this)
        {
            fieldCount += superClass.GetFieldCount();
        }
        return fieldCount;
    }
}