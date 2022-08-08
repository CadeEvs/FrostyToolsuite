using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Assets
{
    [AssetBinding(DataType = typeof(FrostySdk.Ebx.ProfileOptionData))]
    public class ProfileOptionData : Asset, IAssetData<FrostySdk.Ebx.ProfileOptionData>
    {
        public FrostySdk.Ebx.ProfileOptionData Data => data as FrostySdk.Ebx.ProfileOptionData;
        public string OptionType
        {
            get
            {
                if (Data is FrostySdk.Ebx.OptionDataBool) return "Bool";
                else if (Data is FrostySdk.Ebx.OptionDataEnum) return "Enum";
                else if (Data is FrostySdk.Ebx.OptionDataFloat) return "Float";

                return "Unknown";
            }
        }
        public ProfileOptionData(Guid fileGuid, FrostySdk.Ebx.ProfileOptionData inData)
            : base(fileGuid, inData)
        {
        }
    }
}
