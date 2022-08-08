using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Assets
{
#if MASS_EFFECT
    public class CommonAppearanceItem : Asset, IAssetData<FrostySdk.Ebx.CommonAppearanceItemData>
    {
        public FrostySdk.Ebx.CommonAppearanceItemData Data => data as FrostySdk.Ebx.CommonAppearanceItemData;

        public CommonAppearanceItem(Guid fileGuid, FrostySdk.Ebx.CommonAppearanceItemData inData)
            : base(fileGuid, inData)
        {
        }

        public virtual FrostySdk.Ebx.GameObjectData GenerateEntityData()
        {
            return null;
        }
    }
#endif
}
