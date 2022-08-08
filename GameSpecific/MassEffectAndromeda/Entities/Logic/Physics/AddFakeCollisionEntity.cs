using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AddFakeCollisionEntityData))]
	public class AddFakeCollisionEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AddFakeCollisionEntityData>
	{
		public new FrostySdk.Ebx.AddFakeCollisionEntityData Data => data as FrostySdk.Ebx.AddFakeCollisionEntityData;
		public override string DisplayName => "AddFakeCollision";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public AddFakeCollisionEntity(FrostySdk.Ebx.AddFakeCollisionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

