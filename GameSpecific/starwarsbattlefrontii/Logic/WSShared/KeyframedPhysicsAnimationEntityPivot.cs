using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.KeyframedPhysicsAnimationEntityPivotData))]
	public class KeyframedPhysicsAnimationEntityPivot : SimpleRotationEntity, IEntityData<FrostySdk.Ebx.KeyframedPhysicsAnimationEntityPivotData>
	{
		public new FrostySdk.Ebx.KeyframedPhysicsAnimationEntityPivotData Data => data as FrostySdk.Ebx.KeyframedPhysicsAnimationEntityPivotData;
		public override string DisplayName => "KeyframedPhysicsAnimationEntityPivot";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public KeyframedPhysicsAnimationEntityPivot(FrostySdk.Ebx.KeyframedPhysicsAnimationEntityPivotData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

