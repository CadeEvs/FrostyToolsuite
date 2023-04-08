namespace Frosty.Sdk.Managers.Infos;

public struct CasFileInfo
{
    public bool IsPatch
    {
        get => ((m_data >> 16) & 0xFF) != 0;
        set => m_data = (m_data & 0xFF00FFFFu) | (value ? 0x00010000u : 0);
    }
    public int InstallChunkIndex
    {
        get => (int)((m_data >> 8) & 0xFF);
        set => m_data = (m_data & 0xFF00FFFFu) | (uint)((value & 0xFF) << 8);
    }
    public int CasIndex
    {
        get => (int)((m_data >> 0) & 0xFF);
        set => m_data = (m_data & 0xFF00FFFFu) | (uint)(value & 0xFF);
    }

    private uint m_data;

    public CasFileInfo(uint data)
    {
        m_data = data;
    }

    public CasFileInfo(bool isPatch, int installChunkIndex, int casIndex)
    {
        IsPatch = isPatch;
        InstallChunkIndex = installChunkIndex;
        CasIndex = casIndex;
    }
    
    public static implicit operator uint(CasFileInfo value) => value.m_data;
    
    public static implicit operator CasFileInfo(uint value) => new(value);
}