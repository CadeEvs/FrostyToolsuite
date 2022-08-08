using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DrawDebugCylinderEntityData))]
	public class DrawDebugCylinderEntity : DrawDebugBaseEntity, IEntityData<FrostySdk.Ebx.DrawDebugCylinderEntityData>
	{
		public new FrostySdk.Ebx.DrawDebugCylinderEntityData Data => data as FrostySdk.Ebx.DrawDebugCylinderEntityData;
		public override string DisplayName => "DrawDebugCylinder";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DrawDebugCylinderEntity(FrostySdk.Ebx.DrawDebugCylinderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

