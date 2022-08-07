using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.Vec2HubEntityData))]
	public class Vec2HubEntity : LogicEntity, IEntityData<FrostySdk.Ebx.Vec2HubEntityData>
	{
		public new FrostySdk.Ebx.Vec2HubEntityData Data => data as FrostySdk.Ebx.Vec2HubEntityData;
		public override string DisplayName => "Vec2Hub";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public Vec2HubEntity(FrostySdk.Ebx.Vec2HubEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

