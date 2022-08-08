using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.KeyframedPhysicsAnimationEntityGameSplineEntityData))]
	public class KeyframedPhysicsAnimationEntityGameSplineEntity : GameSplineEntity, IEntityData<FrostySdk.Ebx.KeyframedPhysicsAnimationEntityGameSplineEntityData>
	{
		public new FrostySdk.Ebx.KeyframedPhysicsAnimationEntityGameSplineEntityData Data => data as FrostySdk.Ebx.KeyframedPhysicsAnimationEntityGameSplineEntityData;
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public KeyframedPhysicsAnimationEntityGameSplineEntity(FrostySdk.Ebx.KeyframedPhysicsAnimationEntityGameSplineEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

