using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BillboardTransformEntityData))]
	public class BillboardTransformEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.BillboardTransformEntityData>
	{
		public new FrostySdk.Ebx.BillboardTransformEntityData Data => data as FrostySdk.Ebx.BillboardTransformEntityData;
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public BillboardTransformEntity(FrostySdk.Ebx.BillboardTransformEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

