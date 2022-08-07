using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Assets
{
    [AssetBinding(DataType = typeof(FrostySdk.Ebx.LayerData))]
    public class Layer : PrefabBlueprint, IAssetData<FrostySdk.Ebx.LayerData>
    {
        public new FrostySdk.Ebx.LayerData Data => data as FrostySdk.Ebx.LayerData;

        public Layer(Guid fileGuid, FrostySdk.Ebx.LayerData inData)
            : base(fileGuid, inData)
        {
        }
    }
}
