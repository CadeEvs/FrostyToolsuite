using Frosty.Sdk.Sdk.TypeInfoDatas;

namespace Frosty.Sdk.Sdk.TypeInfos;

internal class ArrayInfo : TypeInfo
{
    public ArrayInfo(ArrayInfoData data)
        : base(data)
    {
    }

    public TypeInfo GetTypeInfo() => (m_data as ArrayInfoData)!.GetTypeInfo();
}