using Frosty.Sdk.IO;
using Frosty.Sdk.Sdk.TypeInfos;

namespace Frosty.Sdk.Sdk;

internal class MethodInfo
{
    public FunctionInfo GetFunctionInfo() => (TypeInfo.TypeInfoMapping[p_functionInfo] as FunctionInfo)!;

    public FunctionInfo GetFunctionInfo2() => (TypeInfo.TypeInfoMapping[p_unknown] as FunctionInfo)!;


    private uint m_nameHash;
    private long p_unknown;
    private long p_functionInfo;

    public bool Read(MemoryReader reader)
    {
        m_nameHash = reader.ReadUInt();
        
        if (m_nameHash == 0)
        {
            return false;
        }

        p_unknown = reader.ReadLong();
        p_functionInfo = reader.ReadLong();
        return true;
    }
}