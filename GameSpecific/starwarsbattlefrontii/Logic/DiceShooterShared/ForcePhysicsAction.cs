using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ForcePhysicsActionData))]
	public class ForcePhysicsAction : PhysicsActionBase, IEntityData<FrostySdk.Ebx.ForcePhysicsActionData>
	{
		public new FrostySdk.Ebx.ForcePhysicsActionData Data => data as FrostySdk.Ebx.ForcePhysicsActionData;
		public override string DisplayName => "ForcePhysicsAction";

		public ForcePhysicsAction(FrostySdk.Ebx.ForcePhysicsActionData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

