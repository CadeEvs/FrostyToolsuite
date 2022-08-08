using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Assets
{
    [AssetBinding(DataType = typeof(FrostySdk.Ebx.ActionLogicPrefabBlueprint))]
    public class ActionLogicPrefabBlueprint : AbstractPlotLogicPrefabBlueprint, IAssetData<FrostySdk.Ebx.ActionLogicPrefabBlueprint>
    {
        public new FrostySdk.Ebx.ActionLogicPrefabBlueprint Data => data as FrostySdk.Ebx.ActionLogicPrefabBlueprint;

        public ActionLogicPrefabBlueprint(Guid fileGuid, FrostySdk.Ebx.ActionLogicPrefabBlueprint inData)
            : base(fileGuid, inData)
        {
        }
    }
}
