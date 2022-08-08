using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Assets
{
    [AssetBinding(DataType = typeof(FrostySdk.Ebx.VehicleBlueprint))]
    public class VehicleBlueprint : ObjectBlueprint, IAssetData<FrostySdk.Ebx.VehicleBlueprint>
    {
        public new FrostySdk.Ebx.VehicleBlueprint Data => data as FrostySdk.Ebx.VehicleBlueprint;

        public VehicleBlueprint(Guid fileGuid, FrostySdk.Ebx.VehicleBlueprint inData)
            : base(fileGuid, inData)
        {
        }
    }
}
