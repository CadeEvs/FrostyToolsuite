using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClosestPointEntityData))]
	public class ClosestPointEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.ClosestPointEntityData>
	{
		public new FrostySdk.Ebx.ClosestPointEntityData Data => data as FrostySdk.Ebx.ClosestPointEntityData;
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClosestPointEntity(FrostySdk.Ebx.ClosestPointEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

