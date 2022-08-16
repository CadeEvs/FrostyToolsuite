using System;
using LevelEditorPlugin.Assets;

namespace LevelEditorPlugin.Assets
{
    [AssetBinding(DataType = typeof(FrostySdk.Ebx.ProfileOptionData))]
    public class ProfileOptionData : Asset, IAssetData<FrostySdk.Ebx.ProfileOptionData>
    {
        public new FrostySdk.Ebx.ProfileOptionData Data => data as FrostySdk.Ebx.ProfileOptionData;

        public ProfileOptionData(Guid fileGuid, FrostySdk.Ebx.ProfileOptionData inData)
            : base(fileGuid, inData)
        {
        }
    }
}