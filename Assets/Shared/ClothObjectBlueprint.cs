using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Assets
{
    [AssetBinding(DataType = typeof(FrostySdk.Ebx.ClothObjectBlueprint))]
    public class ClothObjectBlueprint : ObjectBlueprint, IAssetData<FrostySdk.Ebx.ClothObjectBlueprint>
    {
        public new FrostySdk.Ebx.ClothObjectBlueprint Data => data as FrostySdk.Ebx.ClothObjectBlueprint;

        public ClothObjectBlueprint(Guid fileGuid, FrostySdk.Ebx.ClothObjectBlueprint inData)
            : base(fileGuid, inData)
        {
        }
    }
}
