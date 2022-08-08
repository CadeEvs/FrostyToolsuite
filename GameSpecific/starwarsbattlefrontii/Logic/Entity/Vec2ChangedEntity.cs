using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.Vec2ChangedEntityData))]
	public class Vec2ChangedEntity : PropertyChangedEntity, IEntityData<FrostySdk.Ebx.Vec2ChangedEntityData>
	{
		public new FrostySdk.Ebx.Vec2ChangedEntityData Data => data as FrostySdk.Ebx.Vec2ChangedEntityData;
		public override string DisplayName => "Vec2Changed";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public Vec2ChangedEntity(FrostySdk.Ebx.Vec2ChangedEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

