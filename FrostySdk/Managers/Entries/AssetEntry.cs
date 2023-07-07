using System.Collections.Generic;
using Frosty.Sdk.Interfaces;

namespace Frosty.Sdk.Managers.Entries;

public abstract class AssetEntry
{
    /// <summary>
    /// The name of this <see cref="AssetEntry"/>.
    /// </summary>
    public virtual string Name { get; internal set; } = string.Empty;
    
    /// <summary>
    /// The Type of this <see cref="AssetEntry"/>.
    /// </summary>
    public virtual string Type { get; internal set; } = string.Empty;

    /// <summary>
    /// The AssetType of this <see cref="AssetEntry"/>.
    /// </summary>
    public virtual string AssetType => string.Empty;
    
    /// <summary>
    /// The Filename of this <see cref="AssetEntry"/>.
    /// </summary>
    public virtual string Filename
    {
        get
        {
            int id = Name.LastIndexOf('/');
            return id == -1 ? Name : Name[(id + 1)..];
        }
    }
    
    /// <summary>
    /// The Path of this <see cref="AssetEntry"/>.
    /// </summary>
    public virtual string Path
    {
        get
        {
            int id = Name.LastIndexOf('/');
            return id == -1 ? string.Empty : Name[..id];
        }
    }

    /// <summary>
    /// The <see cref="Sha1"/> hash of the compressed data of this <see cref="AssetEntry"/>.
    /// </summary>
    public Sha1 Sha1 { get; internal set; }

    /// <summary>
    /// The size of the compressed data of this <see cref="AssetEntry"/>.
    /// </summary>
    public long Size { get; internal set; }

    /// <summary>
    /// The size of the uncompressed data of this <see cref="AssetEntry"/>.
    /// </summary>
    public long OriginalSize { get; internal set; }

    /// <summary>
    /// The Bundles that contain this <see cref="AssetEntry"/>. 
    /// </summary>
    public readonly HashSet<int> Bundles = new();

    /// <summary>
    /// The <see cref="FileInfos"/> of this <see cref="AssetEntry"/>.
    /// </summary>
    internal readonly HashSet<IFileInfo> FileInfos = new();

    protected AssetEntry(Sha1 inSha1, long inSize, long inOriginalSize)
    {
        Sha1 = inSha1;
        Size = inSize;
        OriginalSize = inOriginalSize;
    }

    /// <summary>
    /// Checks if this <see cref="AssetEntry"/> is in the specified Bundle.
    /// </summary>
    /// <param name="bid">The Id of the Bundle.</param>
    /// <returns></returns>
    public bool IsInBundle(int bid) => Bundles.Contains(bid);

    /// <summary>
    /// Iterates through all bundles that the asset is a part of
    /// </summary>
    public IEnumerable<int> EnumerateBundles() => Bundles;
}