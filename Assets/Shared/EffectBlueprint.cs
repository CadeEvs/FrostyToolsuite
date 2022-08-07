using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Assets
{
    [AssetBinding(DataType = typeof(FrostySdk.Ebx.EffectBlueprint))]
    public class EffectBlueprint : ObjectBlueprint, IAssetData<FrostySdk.Ebx.EffectBlueprint>
    {
        public new FrostySdk.Ebx.EffectBlueprint Data => data as FrostySdk.Ebx.EffectBlueprint;

        public EffectBlueprint(Guid fileGuid, FrostySdk.Ebx.EffectBlueprint inData)
            : base(fileGuid, inData)
        {
        }
    }
}
