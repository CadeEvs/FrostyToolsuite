using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Assets
{
    [AssetBinding(DataType = typeof(FrostySdk.Ebx.LensFlareBlueprint))]
    public class LensFlareBlueprint : ObjectBlueprint, IAssetData<FrostySdk.Ebx.LensFlareBlueprint>
    {
        public new FrostySdk.Ebx.LensFlareBlueprint Data => data as FrostySdk.Ebx.LensFlareBlueprint;

        public LensFlareBlueprint(Guid fileGuid, FrostySdk.Ebx.LensFlareBlueprint inData)
            : base(fileGuid, inData)
        {
        }
    }
}
