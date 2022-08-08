using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.KeyframedPhysicsAnimationEntityData))]
	public class KeyframedPhysicsAnimationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.KeyframedPhysicsAnimationEntityData>
	{
		public new FrostySdk.Ebx.KeyframedPhysicsAnimationEntityData Data => data as FrostySdk.Ebx.KeyframedPhysicsAnimationEntityData;
		public override string DisplayName => "KeyframedPhysicsAnimation";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public KeyframedPhysicsAnimationEntity(FrostySdk.Ebx.KeyframedPhysicsAnimationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

