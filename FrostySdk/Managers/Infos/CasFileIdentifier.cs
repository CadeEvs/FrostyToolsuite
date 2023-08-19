using System;

namespace Frosty.Sdk.Managers.Infos;

public readonly struct CasFileIdentifier
{
    public bool IsPatch { get; }
    public int InstallChunkIndex { get; }
    public int CasIndex { get; }

    public static CasFileIdentifier FromFileIdentifier(uint file)
    {
        return new CasFileIdentifier(((file >> 16) & 0xFF) != 0, (int)((file >> 8) & 0xFF), (int)((file >> 0) & 0xFF));
    }

    public static CasFileIdentifier FromFileIdentifier(uint file1, uint file2)
    {
        return new CasFileIdentifier(((file1 >> 16) & 0xFF) != 0, (int)(((file1 << 16) & 0xFFFF0000) | ((file2 >> 16) & 0xFFFF)), (int)((file2 >> 0) & 0xFFFF));
    }

    public static uint ToFileIdentifier(CasFileIdentifier file)
    {
        return (uint)((file.IsPatch ? 1 << 16 : 0) | (file.InstallChunkIndex << 8) | (file.CasIndex));
    }
    
    public static CasFileIdentifier FromFileIdentifierV2(uint file)
    {
        return new CasFileIdentifier((file & 0x100) != 0, (int)(file >> 12), (int)((file >> 0) & 0xFF) + 1);
    }
    
    public CasFileIdentifier(bool inIsPatch, int inInstallChunkIndex, int inCasIndex)
    {
        IsPatch = inIsPatch;
        InstallChunkIndex = inInstallChunkIndex;
        CasIndex = inCasIndex;
    }

    public static bool operator ==(CasFileIdentifier a, CasFileIdentifier b) => a.Equals(b);
    public static bool operator !=(CasFileIdentifier a, CasFileIdentifier b) => !a.Equals(b);

    public bool Equals(CasFileIdentifier b)
    {
        return IsPatch == b.IsPatch && InstallChunkIndex == b.InstallChunkIndex && CasIndex == b.CasIndex;
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is not CasFileIdentifier b)
        {
            return false;
        }

        return Equals(b);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(IsPatch, InstallChunkIndex, CasIndex);
    }
}