using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Assets
{
    [AssetBinding(DataType = typeof(FrostySdk.Ebx.ObjectBlueprint))]
    public class ObjectBlueprint : Blueprint, IAssetData<FrostySdk.Ebx.ObjectBlueprint>
    {
        public new FrostySdk.Ebx.ObjectBlueprint Data => data as FrostySdk.Ebx.ObjectBlueprint;

        public ObjectBlueprint(Guid fileGuid, FrostySdk.Ebx.ObjectBlueprint inData)
            : base(fileGuid, inData)
        {
        }

        public override FrostySdk.Ebx.ReferenceObjectData CreateEntityData()
        {
            // @hack: this will not work for when wanting to create real entity datas
            return new FrostySdk.Ebx.ObjectReferenceObjectData() { Blueprint = ToPointerRef() };
        }
    }
}
