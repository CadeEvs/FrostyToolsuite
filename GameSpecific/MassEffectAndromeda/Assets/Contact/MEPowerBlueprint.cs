using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Assets
{
    [AssetBinding(DataType = typeof(FrostySdk.Ebx.MEPowerBlueprint))]
    public class MEPowerBlueprint : Blueprint, IAssetData<FrostySdk.Ebx.MEPowerBlueprint>
    {
        public new FrostySdk.Ebx.MEPowerBlueprint Data => data as FrostySdk.Ebx.MEPowerBlueprint;

        public MEPowerBlueprint(Guid fileGuid, FrostySdk.Ebx.MEPowerBlueprint inData)
            : base(fileGuid, inData)
        {
        }
    }
}
