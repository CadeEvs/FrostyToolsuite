using System;
using System.Collections.Generic;
using FrostySdk.Managers;
using FrostySdk.Managers.Entries;

namespace AnimationEditorPlugin.Managers
{
    public class AssetBankFileEntry : AssetEntry
    {
        // @TODO: Fill out name, type, and guid once data is added
        
        public override string AssetType => "assetbank";
        
        public List<ResInstance> ResInstances = new List<ResInstance>();
        
        public class ResInstance
        {
            public ResInstance ModifiedEntry { get; set; }
            public bool IsModified => ModifiedEntry != null;

            public ResAssetEntry Entry;
            public Guid ChunkId;
            public long Offset;
            public long Size;
        }
    }
}