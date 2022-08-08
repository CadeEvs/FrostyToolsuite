using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Assets
{
#if MASS_EFFECT
    [AssetBinding(DataType = typeof(FrostySdk.Ebx.GameObjectBlueprint))]
    public class GameObjectBlueprint : ObjectBlueprint, IAssetData<FrostySdk.Ebx.GameObjectBlueprint>
    {
        public new FrostySdk.Ebx.GameObjectBlueprint Data => data as FrostySdk.Ebx.GameObjectBlueprint;

        public GameObjectBlueprint(Guid fileGuid, FrostySdk.Ebx.GameObjectBlueprint inData)
            : base(fileGuid, inData)
        {
        }
    }
#endif
}
