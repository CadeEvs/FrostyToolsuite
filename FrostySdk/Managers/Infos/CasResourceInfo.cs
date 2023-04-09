using Frosty.Sdk.Interfaces;

namespace Frosty.Sdk.Managers.Infos;

public struct CasResourceInfo
{
    public CasFileInfo CasFileInfo;
    public long Offset;
    public long Size;

    public CasResourceInfo(CasFileInfo casFileInfo, long offset, long size)
    {
        CasFileInfo = casFileInfo;
        Offset = offset;
        Size = size;
    }

    public CasResourceInfo(bool isPatch, int installChunkIndex, int casIndex, long offset, long size)
    {
        CasFileInfo = new CasFileInfo(isPatch, installChunkIndex, casIndex);
        Offset = offset;
        Size = size;
    }
}