using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Assets
{
    [AssetBinding(DataType = typeof(FrostySdk.Ebx.MEProjectileBlueprint))]
    public class MEProjectileBlueprint : ObjectBlueprint, IAssetData<FrostySdk.Ebx.MEProjectileBlueprint>
    {
        public new FrostySdk.Ebx.MEProjectileBlueprint Data => data as FrostySdk.Ebx.MEProjectileBlueprint;

        public MEProjectileBlueprint(Guid fileGuid, FrostySdk.Ebx.MEProjectileBlueprint inData)
            : base(fileGuid, inData)
        {
        }
    }
}
