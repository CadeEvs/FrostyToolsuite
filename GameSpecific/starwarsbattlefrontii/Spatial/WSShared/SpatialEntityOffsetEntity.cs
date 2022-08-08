using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpatialEntityOffsetEntityData))]
	public class SpatialEntityOffsetEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.SpatialEntityOffsetEntityData>
	{
		public new FrostySdk.Ebx.SpatialEntityOffsetEntityData Data => data as FrostySdk.Ebx.SpatialEntityOffsetEntityData;
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SpatialEntityOffsetEntity(FrostySdk.Ebx.SpatialEntityOffsetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

