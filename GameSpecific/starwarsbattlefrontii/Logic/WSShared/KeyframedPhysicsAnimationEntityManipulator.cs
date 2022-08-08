using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.KeyframedPhysicsAnimationEntityManipulatorData))]
	public class KeyframedPhysicsAnimationEntityManipulator : KeyframedPhysicsAnimationEntityPivot, IEntityData<FrostySdk.Ebx.KeyframedPhysicsAnimationEntityManipulatorData>
	{
		public new FrostySdk.Ebx.KeyframedPhysicsAnimationEntityManipulatorData Data => data as FrostySdk.Ebx.KeyframedPhysicsAnimationEntityManipulatorData;
		public override string DisplayName => "KeyframedPhysicsAnimationEntityManipulator";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public KeyframedPhysicsAnimationEntityManipulator(FrostySdk.Ebx.KeyframedPhysicsAnimationEntityManipulatorData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

