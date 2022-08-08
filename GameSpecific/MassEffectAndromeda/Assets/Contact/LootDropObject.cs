using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Assets
{
    [AssetBinding(DataType = typeof(FrostySdk.Ebx.LootDropObject))]
    public class LootDropObject : Asset, IAssetData<FrostySdk.Ebx.LootDropObject>
    {
        public FrostySdk.Ebx.LootDropObject Data => data as FrostySdk.Ebx.LootDropObject;

        public LootDropObject(Guid fileGuid, FrostySdk.Ebx.LootDropObject inData)
            : base(fileGuid, inData)
        {
        }
    }
}
