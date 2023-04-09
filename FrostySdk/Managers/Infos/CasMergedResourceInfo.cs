namespace Frosty.Sdk.Managers.Infos;

public struct CasMergedResourceInfo
{
    public CasMergedFileInfo CasMergedFileInfo
    {
        get => m_file;
        set => m_file = value;
    }

    public int FileIndex
    {
        get => (int)m_file;
        set => m_file = (uint)value;
    }
    public long Offset;
    public long Size;

    private uint m_file;

    public CasMergedResourceInfo(CasMergedFileInfo casMergedFileInfo, long offset, long size)
    {
        CasMergedFileInfo = casMergedFileInfo;
        Offset = offset;
        Size = size;
    }

    public CasMergedResourceInfo(bool isPatch, int installChunkIndex, int casIndex, long offset, long size)
    {
        CasMergedFileInfo = new CasMergedFileInfo(isPatch, installChunkIndex, casIndex);
        Offset = offset;
        Size = size;
    }
    
    public CasMergedResourceInfo(int fileIndex, long offset, long size)
    {
        FileIndex = fileIndex;
        Offset = offset;
        Size = size;
    }
}