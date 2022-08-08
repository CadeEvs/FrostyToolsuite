using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VelocityOnInputPhysicsActionData))]
	public class VelocityOnInputPhysicsAction : PhysicsActionBase, IEntityData<FrostySdk.Ebx.VelocityOnInputPhysicsActionData>
	{
		public new FrostySdk.Ebx.VelocityOnInputPhysicsActionData Data => data as FrostySdk.Ebx.VelocityOnInputPhysicsActionData;
		public override string DisplayName => "VelocityOnInputPhysicsAction";

		public VelocityOnInputPhysicsAction(FrostySdk.Ebx.VelocityOnInputPhysicsActionData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

