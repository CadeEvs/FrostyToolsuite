using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RopeColliderEntityData))]
	public class RopeColliderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.RopeColliderEntityData>
	{
		public new FrostySdk.Ebx.RopeColliderEntityData Data => data as FrostySdk.Ebx.RopeColliderEntityData;
		public override string DisplayName => "RopeCollider";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public RopeColliderEntity(FrostySdk.Ebx.RopeColliderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

