using System;
using System.Collections.Generic;
using AnimationEditorPlugin.Formats;
using FrostySdk.Managers;
using FrostySdk.Managers.Entries;

namespace AnimationEditorPlugin.Managers
{
    public class AssetBankFileEntry : AssetEntry
    {
        // @TODO: This is all temporary just to view asset bank types
        public override string Name => Bank != null ? $"{Bank.Name}/{Bank.Name}" : "";
        public override string Type => Bank != null ? Bank.Name : "";

        public override string AssetType => "assetbank";
        
        public List<ResInstance> ResInstances = new List<ResInstance>();

        public Bank Bank;
        
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