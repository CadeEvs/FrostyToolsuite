using System;

namespace Frosty.Sdk.Ebx;

public readonly struct AssetClassGuid
{
    public Guid ExportedGuid => m_exportedGuid;
    public int InternalId => m_internalId;
    public bool IsExported => m_isExported;

    private readonly Guid m_exportedGuid;
    private readonly int m_internalId;
    private readonly bool m_isExported;

    public AssetClassGuid(Guid inGuid, int inId)
    {
        m_exportedGuid = inGuid;
        m_internalId = inId;
        m_isExported = (inGuid != Guid.Empty);
    }

    public AssetClassGuid(int inId)
    {
        m_exportedGuid = Guid.Empty;
        m_internalId = inId;
        m_isExported = false;
    }

    public static bool operator ==(AssetClassGuid a, object b) => a.Equals(b);

    public static bool operator !=(AssetClassGuid a, object b) => !a.Equals(b);

    public override bool Equals(object? obj)
    {
        switch (obj)
        {
            case null:
                return false;
            case AssetClassGuid reference:
                return (m_isExported == reference.m_isExported && m_exportedGuid == reference.m_exportedGuid && m_internalId == reference.m_internalId);
            case Guid guid:
                return (m_isExported && guid == m_exportedGuid);
            case int id:
                return (m_internalId == id);
            default:
                return false;
        }
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;
            hash = (hash * 16777619) ^ m_exportedGuid.GetHashCode();
            hash = (hash * 16777619) ^ m_internalId.GetHashCode();
            hash = (hash * 16777619) ^ m_isExported.GetHashCode();
            return hash;
        }
    }

    public override string ToString() => m_isExported ? m_exportedGuid.ToString() : $"00000000-0000-0000-0000-{m_internalId:x12}";
}