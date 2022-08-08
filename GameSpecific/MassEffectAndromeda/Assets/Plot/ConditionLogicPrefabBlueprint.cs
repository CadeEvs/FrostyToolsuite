using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Assets
{
    [AssetBinding(DataType = typeof(FrostySdk.Ebx.ConditionLogicPrefabBlueprint))]
    public class ConditionLogicPrefabBlueprint : AbstractPlotLogicPrefabBlueprint, IAssetData<FrostySdk.Ebx.ConditionLogicPrefabBlueprint>
    {
        public new FrostySdk.Ebx.ConditionLogicPrefabBlueprint Data => data as FrostySdk.Ebx.ConditionLogicPrefabBlueprint;

        public ConditionLogicPrefabBlueprint(Guid fileGuid, FrostySdk.Ebx.ConditionLogicPrefabBlueprint inData)
            : base(fileGuid, inData)
        {
        }
    }
}
