using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Assets
{
    [AssetBinding(DataType = typeof(FrostySdk.Ebx.UIWidgetBlueprint))]
    public class UIWidgetBlueprint : ObjectBlueprint, IAssetData<FrostySdk.Ebx.UIWidgetBlueprint>
    {
        public new FrostySdk.Ebx.UIWidgetBlueprint Data => data as FrostySdk.Ebx.UIWidgetBlueprint;

        public UIWidgetBlueprint(Guid fileGuid, FrostySdk.Ebx.UIWidgetBlueprint inData)
            : base(fileGuid, inData)
        {
        }
    }
}
