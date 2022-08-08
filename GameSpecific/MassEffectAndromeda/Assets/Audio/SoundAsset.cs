using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Assets
{
    [AssetBinding(DataType = typeof(FrostySdk.Ebx.SoundAsset))]
    public class SoundAsset : Asset, IAssetData<FrostySdk.Ebx.SoundAsset>
    {
        public FrostySdk.Ebx.SoundAsset Data => data as FrostySdk.Ebx.SoundAsset;

        public SoundAsset(Guid fileGuid, FrostySdk.Ebx.SoundAsset inData)
            : base(fileGuid, inData)
        {
        }
    }
}
