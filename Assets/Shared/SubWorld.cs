using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Assets
{
    [AssetBinding(DataType = typeof(FrostySdk.Ebx.SubWorldData))]
    public class SubWorld : SpatialPrefabBlueprint, IAssetData<FrostySdk.Ebx.SubWorldData>
    {
        public new FrostySdk.Ebx.SubWorldData Data => data as FrostySdk.Ebx.SubWorldData;

        public SubWorld(Guid fileGuid, FrostySdk.Ebx.SubWorldData inData)
            : base(fileGuid, inData)
        {
        }
    }
}
