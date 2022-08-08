using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Assets
{
    [AssetBinding(DataType = typeof(FrostySdk.Ebx.VisualEnvironmentBlueprint))]
    public class VisualEnvironmentBlueprint : ObjectBlueprint, IAssetData<FrostySdk.Ebx.VisualEnvironmentBlueprint>
    {
        public new FrostySdk.Ebx.VisualEnvironmentBlueprint Data => data as FrostySdk.Ebx.VisualEnvironmentBlueprint;

        public VisualEnvironmentBlueprint(Guid fileGuid, FrostySdk.Ebx.VisualEnvironmentBlueprint inData)
            : base(fileGuid, inData)
        {
        }
    }
}
