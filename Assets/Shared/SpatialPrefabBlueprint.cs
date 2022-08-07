using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Assets
{
    [AssetBinding(DataType = typeof(FrostySdk.Ebx.SpatialPrefabBlueprint))]
    public class SpatialPrefabBlueprint : PrefabBlueprint, IAssetData<FrostySdk.Ebx.SpatialPrefabBlueprint>
    {
        public new FrostySdk.Ebx.SpatialPrefabBlueprint Data => data as FrostySdk.Ebx.SpatialPrefabBlueprint;

        public SpatialPrefabBlueprint(Guid fileGuid, FrostySdk.Ebx.SpatialPrefabBlueprint inData)
            : base(fileGuid, inData)
        {
        }

        public override FrostySdk.Ebx.ReferenceObjectData CreateEntityData()
        {
            // @hack: this will not work for when wanting to create real entity datas
            return new FrostySdk.Ebx.SpatialPrefabReferenceObjectData() { Blueprint = ToPointerRef() };
        }
    }
}
