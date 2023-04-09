namespace Frosty.Sdk.Managers.Infos;

public struct FileInfo
{
    public string Path = string.Empty;
    public long Offset;
    public long Size;

    public FileInfo(string inPath, long inOffset, long inSize)
    {
        Path = inPath;
        Offset = inOffset;
        Size = inSize;
    }
}