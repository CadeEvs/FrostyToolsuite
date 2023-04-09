using Frosty.Sdk.Interfaces;

namespace Frosty.Sdk.Managers.Infos;

public struct CasMergedFileInfo : ICasFileInfo
{
    public bool GetIsPatch() => ((m_data >> 8) & 1) != 0;
    public void SetIsPatch(bool value) => m_data = (m_data & 0x100) | (value ? 0x100u : 0);
    
    public int GetInstallChunkIndex() => (int)(m_data >> 12);
    public void SetInstallChunkIndex(int value) => m_data = (m_data & 0xFF00FFFFu) | (uint)((value & 0xFF) << 8);
    
    public int GetCasIndex() => (int)((m_data >> 0) & 0xFF) + 1;
    public void SetCasIndex(int value) => m_data = (m_data & 0xFF00FFFFu) | (uint)((value - 1) & 0xFF);
    
    private uint m_data;

    public CasMergedFileInfo(uint data)
    {
        m_data = data;
    }

    public CasMergedFileInfo(bool isPatch, int installChunkIndex, int casIndex)
    {
        SetIsPatch(isPatch);
        SetInstallChunkIndex(installChunkIndex);
        SetCasIndex(casIndex);
    }
    
    public static implicit operator uint(CasMergedFileInfo value) => value.m_data;
    
    public static implicit operator CasMergedFileInfo(uint value) => new(value);
}