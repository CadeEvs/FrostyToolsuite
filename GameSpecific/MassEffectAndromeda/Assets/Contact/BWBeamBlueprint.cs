using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Assets
{
    [AssetBinding(DataType = typeof(FrostySdk.Ebx.BWBeamBlueprint))]
    public class BWBeamBlueprint : ObjectBlueprint, IAssetData<FrostySdk.Ebx.BWBeamBlueprint>
    {
        public new FrostySdk.Ebx.BWBeamBlueprint Data => data as FrostySdk.Ebx.BWBeamBlueprint;

        public BWBeamBlueprint(Guid fileGuid, FrostySdk.Ebx.BWBeamBlueprint inData)
            : base(fileGuid, inData)
        {
        }
    }
}
