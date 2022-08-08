using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Assets
{
#if MASS_EFFECT
    [AssetBinding(DataType = typeof(FrostySdk.Ebx.MESoldierBlueprint))]
    public class MESoldierBlueprint : ObjectBlueprint, IAssetData<FrostySdk.Ebx.MESoldierBlueprint>
    {
        public new FrostySdk.Ebx.MESoldierBlueprint Data => data as FrostySdk.Ebx.MESoldierBlueprint;

        public MESoldierBlueprint(Guid fileGuid, FrostySdk.Ebx.MESoldierBlueprint inData)
            : base(fileGuid, inData)
        {
        }
    }
#endif
}
