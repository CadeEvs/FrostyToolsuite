using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AntiGravityPhysicsActionData))]
	public class AntiGravityPhysicsAction : PhysicsActionBase, IEntityData<FrostySdk.Ebx.AntiGravityPhysicsActionData>
	{
		public new FrostySdk.Ebx.AntiGravityPhysicsActionData Data => data as FrostySdk.Ebx.AntiGravityPhysicsActionData;
		public override string DisplayName => "AntiGravityPhysicsAction";

		public AntiGravityPhysicsAction(FrostySdk.Ebx.AntiGravityPhysicsActionData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

