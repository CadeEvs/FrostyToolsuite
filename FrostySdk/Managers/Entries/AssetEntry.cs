using System;
using System.Collections.Generic;
using Frosty.Sdk.Managers.Infos;
using static Frosty.Sdk.Utils.Utils;

namespace Frosty.Sdk.Managers.Entries;

public enum AssetDataLocation
{
    Cas,
    CasNonIndexed
}

public class AssetEntry
{
    public virtual string Name { get; set; } = string.Empty;
    public virtual string Type { get; set; } = string.Empty;
    public virtual string AssetType { get; } = string.Empty;

    public virtual string DisplayName => $"{Filename}{(IsDirty ? "*" : "")}";
    public virtual string Filename
    {
        get
        {
            int id = Name.LastIndexOf('/');
            return id == -1 ? Name : Name[(id + 1)..];
        }
    }
    public virtual string Path
    {
        get
        {
            int id = Name.LastIndexOf('/');
            return id == -1 ? "" : Name[..id];
        }
    }

    public Sha1 Sha1;
    public long Size;
    public long OriginalSize;
    
    // We dont need this, since an asset is always inline if size < 256
    //public bool IsInline;
    
    public AssetDataLocation Location;
    
    public Sha1 BaseSha1;
    public Sha1 DeltaSha1;

    public FileInfo FileInfo;

    public readonly List<int> Bundles = new();
    
    public readonly List<int> AddedBundles = new();
    public readonly List<int> RemovedBundles = new();

    public ModifiedAssetEntry? ModifiedEntry;
    public readonly List<AssetEntry> LinkedAssets = new();

    private bool m_dirty;
    
    /// <summary>
    /// returns true if this asset was added
    /// </summary>
    public bool IsAdded { get; set; }

    /// <summary>
    /// returns true if this asset or any asset linked to it is modified
    /// </summary>
    public virtual bool IsModified => IsDirectlyModified || IsIndirectlyModified;

    /// <summary>
    /// returns true if this asset (and only this asset) is modified
    /// </summary>
    public bool IsDirectlyModified => ModifiedEntry != null || AddedBundles.Count != 0 || RemovedBundles.Count != 0;

    /// <summary>
    /// 
    /// </summary>
    public bool HasModifiedData => ModifiedEntry != null && (ModifiedEntry.Data != null || ModifiedEntry.DataObject != null);

    /// <summary>
    /// returns true if this asset is considered modified through another linked asset
    /// ie. An ebx would be considered modified if its linked resource has been modified
    /// </summary>
    public bool IsIndirectlyModified
    {
        get
        {
            foreach (AssetEntry entry in LinkedAssets)
            {
                if (entry.IsModified)
                    return true;
            }
            return false;
        }
    }

    /// <summary>
    /// returns true if this asset, or any asset linked to it is dirty
    /// </summary>
    public virtual bool IsDirty
    {
        get
        {
            if (m_dirty)
            {
                return true;
            }

            foreach (AssetEntry entry in LinkedAssets)
            {
                if (entry.IsDirty)
                {
                    return true;
                }
            }
            return false;
        }
        set
        {
            if (m_dirty != value)
            {
                m_dirty = value;
                if (m_dirty)
                {
                    OnModified();
                }
            }
        }
    }

    public AssetEntry(Sha1 inSha1, long inSize, long inOriginalSize)
    {
        Sha1 = inSha1;
        Size = inSize;
        OriginalSize = inOriginalSize;
        BaseSha1 = ResourceManager.GetBaseSha1(inSha1);
    }

    /// <summary>
    /// Links the current asset to another
    /// </summary>
    public void LinkAsset(AssetEntry assetToLink)
    {
        if (!LinkedAssets.Contains(assetToLink))
        {
            LinkedAssets.Add(assetToLink);
        }

        if (assetToLink is ChunkAssetEntry entry)
        {
            if (entry.HasModifiedData)
            {
                // store the res/ebx name in the chunk
                entry.ModifiedEntry!.H32 = HashString(Name, true);
            }
            else
            {
                // asset was added to bundle (so no ModifiedEntry)
                entry.H32 = HashString(Name, true);
            }
        }
    }

    /// <summary>
    /// Adds the current asset to the specified bundle
    /// </summary>
    public virtual bool AddToBundle(int bid)
    {
        if (IsInBundle(bid))
        {
            return false;
        }

        AddedBundles.Add(bid);
        IsDirty = true;

        return true;
    }

    public virtual bool RemoveFromBundle(int bid)
    {
        if (!IsInBundle(bid))
        {
            return false;
        }
        
        if (AddedBundles.Contains(bid))
        {
            AddedBundles.Remove(bid);
        }
        else
        {
            RemovedBundles.Add(bid);
        }

        IsDirty = true;
        
        return true;
    }

    /// <summary>
    /// Adds the current asset to the specified bundles
    /// </summary>
    public bool AddToBundles(IEnumerable<int> bundles)
    {
        bool added = false;
        foreach (int bid in bundles)
        {
            added |= AddToBundle(bid);
        }

        return added;
    }

    /// <summary>
    /// Returns true if asset is in the specified bundle
    /// </summary>
    public bool IsInBundle(int bid) => Bundles.Contains(bid) || AddedBundles.Contains(bid);

    /// <summary>
    /// Iterates through all bundles that the asset is a part of
    /// </summary>
    public IEnumerable<int> EnumerateBundles(bool addedOnly = false)
    {
        if (!addedOnly)
        {
            for (int i = 0; i < Bundles.Count; i++)
            {
                if (!RemovedBundles.Contains(Bundles[i]))
                {
                    yield return Bundles[i];
                }
            }
        }

        for (int i = 0; i < AddedBundles.Count; i++)
        {
            yield return AddedBundles[i];
        }
    }

    public virtual void ClearModifications() => ModifiedEntry = null;

    public event EventHandler? AssetModified;
    public void OnModified() => AssetModified?.Invoke(this, EventArgs.Empty);
}