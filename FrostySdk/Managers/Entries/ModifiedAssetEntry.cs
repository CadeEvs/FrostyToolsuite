using System;
using System.Collections.Generic;

namespace FrostySdk.Managers.Entries
{
    public class ModifiedAssetEntry
    {
        public Sha1 Sha1;
        public byte[] Data;
        public object DataObject;
        public long OriginalSize;

        public byte[] ResMeta;

        public uint LogicalOffset;
        public uint LogicalSize;
        public uint RangeStart;
        public uint RangeEnd;
        public int FirstMip = -1;

        public bool IsInline;

        public bool AddToChunkBundle = true;
        public bool IsTransientModified = false;
        public int H32;

        public List<Guid> DependentAssets = new List<Guid>();
        public string UserData = "";
    }
}