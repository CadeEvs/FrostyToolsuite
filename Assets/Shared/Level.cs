using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Assets
{
    [AssetBinding(DataType = typeof(FrostySdk.Ebx.LevelData))]
    public class Level : SubWorld, IAssetData<FrostySdk.Ebx.LevelData>
    {
        public new FrostySdk.Ebx.LevelData Data => data as FrostySdk.Ebx.LevelData;

        public Level(Guid fileGuid, FrostySdk.Ebx.LevelData inData)
            : base(fileGuid, inData)
        {
        }
    }
}
