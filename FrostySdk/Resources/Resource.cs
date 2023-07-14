using Frosty.Sdk.IO;
using Frosty.Sdk.Managers.Entries;

namespace Frosty.Sdk.Resources;

public class Resource
{
    public ulong ResourceId => m_resRid;
    public byte[] ResourceMeta => m_resMeta;

    protected byte[] m_resMeta;
    protected ulong m_resRid;

    public Resource()
    {
        m_resMeta = new byte[16];
        m_resRid = 0;
    }

    public virtual void Set(ResAssetEntry entry)
    {
        m_resMeta = entry.ResMeta;
        m_resRid = entry.ResRid;
    }
    
    public virtual void Deserialize(DataStream stream)
    {
    }

    public virtual void Serialize(DataStream stream)
    {
    }
}