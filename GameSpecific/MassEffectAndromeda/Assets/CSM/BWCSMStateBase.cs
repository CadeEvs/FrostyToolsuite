using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Assets
{
    [AssetBinding(DataType = typeof(FrostySdk.Ebx.BWCSMStateBase))]
    public class BWCSMStateBase : Asset, IAssetData<FrostySdk.Ebx.BWCSMStateBase>
    {
        public FrostySdk.Ebx.BWCSMStateBase Data => data as FrostySdk.Ebx.BWCSMStateBase;

        public BWCSMStateBase(Guid fileGuid, FrostySdk.Ebx.BWCSMStateBase inData)
            : base(fileGuid, inData)
        {
        }
    }
}
