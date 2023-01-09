using Frosty.Core.IO;
using Frosty.Hash;
using FrostySdk;
using FrostySdk.IO;
using FrostySdk.Managers;
using System.Collections.Generic;
using FrostySdk.Managers.Entries;

namespace Frosty.Core.Mod
{
    public class BaseModResource
    {
        public virtual ModResourceType Type => ModResourceType.Invalid;
        public int ResourceIndex => resourceIndex;
        public string Name => name;
        public Sha1 Sha1 => sha1;
        public long Size => size;
        public int Handler => handlerHash;
        public string UserData => userData;

        public bool IsModified => resourceIndex != -1 && Type != ModResourceType.Embedded && Type != ModResourceType.Bundle;

        public bool ShouldInline => (flags & 0x01) != 0;
        public bool IsTocChunk => (flags & 0x02) != 0;
        public bool HasHandler => handlerHash != 0;
        public bool IsAdded
        {
            get => (flags & 0x08) != 0;
            set => flags |= 0x08;
        }

        public IEnumerable<int> AddedBundles => bundlesToAdd;

        protected int resourceIndex = -1;
        protected string name;
        protected Sha1 sha1;
        protected long size;
        protected byte flags;
        protected int handlerHash;
        protected string userData = "";
        protected List<int> bundlesToAdd = new List<int>();

        internal BaseModResource()
        {
        }

        public BaseModResource(AssetEntry entry)
        {
            if (entry.IsAdded)
                flags |= 0x08;

            foreach (int bundleId in entry.EnumerateBundles(addedOnly: true))
            {
                BundleEntry bentry = App.AssetManager.GetBundleEntry(bundleId);
                AddBundle(bentry.Name.ToLower());
            }
        }

        public virtual void Read(NativeReader reader)
        {
            resourceIndex = reader.ReadInt();
            if (((reader as FrostyModReader).Version <= 3 && resourceIndex != -1) || (reader as FrostyModReader).Version > 3)
                name = reader.ReadNullTerminatedString();

            int count = 0;
            if (resourceIndex != -1)
            {
                sha1 = reader.ReadSha1();
                size = reader.ReadLong();
                flags = reader.ReadByte();
                handlerHash = reader.ReadInt();

                userData = "";
                if ((reader as FrostyModReader).Version >= 3)
                    userData = reader.ReadNullTerminatedString();
            }

            // prior to version 4, mods stored bundles the asset already existed in for modification
            // so must read and ignore this list

            if ((reader as FrostyModReader).Version <= 3 && resourceIndex != -1)
            {
                count = reader.ReadInt();
                for (int i = 0; i < count; i++)
                    reader.ReadInt();

                count = reader.ReadInt();
                for (int i = 0; i < count; i++)
                    bundlesToAdd.Add(reader.ReadInt());
            }

            // as of version 4, only bundles the asset will be added to are stored, existing bundles
            // are extracted from the asset manager during the apply process

            else if ((reader as FrostyModReader).Version > 3)
            {
                count = reader.ReadInt();
                for (int i = 0; i < count; i++)
                    bundlesToAdd.Add(reader.ReadInt());
            }
        }

        public virtual void FillAssetEntry(object entry)
        {
            AssetEntry assetEntry = entry as AssetEntry;
            assetEntry.Name = name;
            assetEntry.Sha1 = sha1;
            assetEntry.OriginalSize = size;
            assetEntry.IsInline = ShouldInline;
        }

        public void ClearAddedBundles()
        {
            bundlesToAdd.Clear();
        }

        public void AddBundle(BundleEntry bentry)
        {
            AddBundle(bentry.Name);
        }
        internal void AddBundle(string name)
        {
            int hash = Fnv1.HashString(name.ToLower());

            if (name.Length == 8 && int.TryParse(name, System.Globalization.NumberStyles.HexNumber, null, out int tmp))
                hash = tmp;

            bundlesToAdd.Add(hash);
        }
    }
}
