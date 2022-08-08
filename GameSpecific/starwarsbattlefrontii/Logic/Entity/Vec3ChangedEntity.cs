using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.Vec3ChangedEntityData))]
	public class Vec3ChangedEntity : PropertyChangedEntity, IEntityData<FrostySdk.Ebx.Vec3ChangedEntityData>
	{
		public new FrostySdk.Ebx.Vec3ChangedEntityData Data => data as FrostySdk.Ebx.Vec3ChangedEntityData;
		public override string DisplayName => "Vec3Changed";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public Vec3ChangedEntity(FrostySdk.Ebx.Vec3ChangedEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

