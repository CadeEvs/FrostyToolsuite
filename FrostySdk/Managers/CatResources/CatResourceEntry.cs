namespace Frosty.Sdk.Managers.CatResources;

public struct CatResourceEntry
{
    public Sha1 Sha1;
    public uint Offset;
    public uint Size;
    public uint LogicalOffset;
    public int ArchiveIndex;

    public bool IsEncrypted;
    public uint OriginalSize;
    public string KeyId;
    public byte[] Checksum;
}