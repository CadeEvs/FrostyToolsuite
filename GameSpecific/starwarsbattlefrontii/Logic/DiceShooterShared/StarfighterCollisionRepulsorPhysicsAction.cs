using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StarfighterCollisionRepulsorPhysicsActionData))]
	public class StarfighterCollisionRepulsorPhysicsAction : PhysicsActionBase, IEntityData<FrostySdk.Ebx.StarfighterCollisionRepulsorPhysicsActionData>
	{
		public new FrostySdk.Ebx.StarfighterCollisionRepulsorPhysicsActionData Data => data as FrostySdk.Ebx.StarfighterCollisionRepulsorPhysicsActionData;
		public override string DisplayName => "StarfighterCollisionRepulsorPhysicsAction";

		public StarfighterCollisionRepulsorPhysicsAction(FrostySdk.Ebx.StarfighterCollisionRepulsorPhysicsActionData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

