using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Assets
{
    [AssetBinding(DataType = typeof(FrostySdk.Ebx.WorldPartData))]
    public class WorldPart : PrefabBlueprint, IAssetData<FrostySdk.Ebx.WorldPartData>
    {
        public new FrostySdk.Ebx.WorldPartData Data => data as FrostySdk.Ebx.WorldPartData;

        public WorldPart(Guid fileGuid, FrostySdk.Ebx.WorldPartData inData)
            : base(fileGuid, inData)
        {
        }
    }
}
