using Frosty.Sdk.Interfaces;

namespace Frosty.Sdk.Managers.Infos;

public struct CasFileInfo : ICasFileInfo
{
    public bool GetIsPatch() => ((m_data >> 16) & 0xFF) != 0;
    public void SetIsPatch(bool value) => m_data = (m_data & 0xFF00FFFFu) | (value ? 0x00010000u : 0);
    
    public int GetInstallChunkIndex() => (int)((m_data >> 8) & 0xFF);
    public void SetInstallChunkIndex(int value) => m_data = (m_data & 0xFFFF00FFu) | (uint)((value & 0xFF) << 8);
    
    public int GetCasIndex() => (int)((m_data >> 0) & 0xFF);
    public void SetCasIndex(int value) => m_data = (m_data & 0xFFFFFF00u) | (uint)(value & 0xFF);

    private uint m_data;

    public CasFileInfo(uint data)
    {
        m_data = data;
    }

    public CasFileInfo(bool isPatch, int installChunkIndex, int casIndex)
    {
        SetIsPatch(isPatch);
        SetInstallChunkIndex(installChunkIndex);
        SetCasIndex(casIndex);
    }
    
    public static implicit operator uint(CasFileInfo value) => value.m_data;
    
    public static implicit operator CasFileInfo(uint value) => new(value);
}