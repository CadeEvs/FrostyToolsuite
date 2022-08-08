using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Assets
{
    public abstract class AbstractPlotLogicPrefabBlueprint : LogicPrefabBlueprint, IAssetData<FrostySdk.Ebx.AbstractPlotLogicPrefabBlueprint>
    {
        public new FrostySdk.Ebx.AbstractPlotLogicPrefabBlueprint Data => data as FrostySdk.Ebx.AbstractPlotLogicPrefabBlueprint;

        public AbstractPlotLogicPrefabBlueprint(Guid fileGuid, FrostySdk.Ebx.AbstractPlotLogicPrefabBlueprint inData)
            : base(fileGuid, inData)
        {
        }
    }
}
