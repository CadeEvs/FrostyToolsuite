using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LinearRepulsorLiftPhysicsActionData))]
	public class LinearRepulsorLiftPhysicsAction : PhysicsActionBase, IEntityData<FrostySdk.Ebx.LinearRepulsorLiftPhysicsActionData>
	{
		public new FrostySdk.Ebx.LinearRepulsorLiftPhysicsActionData Data => data as FrostySdk.Ebx.LinearRepulsorLiftPhysicsActionData;
		public override string DisplayName => "LinearRepulsorLiftPhysicsAction";

		public LinearRepulsorLiftPhysicsAction(FrostySdk.Ebx.LinearRepulsorLiftPhysicsActionData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

