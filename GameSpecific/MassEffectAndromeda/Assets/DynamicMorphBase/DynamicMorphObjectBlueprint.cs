using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Assets
{
#if MASS_EFFECT
    [AssetBinding(DataType = typeof(FrostySdk.Ebx.DynamicMorphObjectBlueprint))]
    public class DynamicMorphObjectBlueprint : ObjectBlueprint, IAssetData<FrostySdk.Ebx.DynamicMorphObjectBlueprint>
    {
        public new FrostySdk.Ebx.DynamicMorphObjectBlueprint Data => data as FrostySdk.Ebx.DynamicMorphObjectBlueprint;

        public DynamicMorphObjectBlueprint(Guid fileGuid, FrostySdk.Ebx.DynamicMorphObjectBlueprint inData)
            : base(fileGuid, inData)
        {
        }
    }
#endif
}
