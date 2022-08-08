using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HoverLiftPhysicsActionData))]
	public class HoverLiftPhysicsAction : PhysicsActionBase, IEntityData<FrostySdk.Ebx.HoverLiftPhysicsActionData>
	{
		public new FrostySdk.Ebx.HoverLiftPhysicsActionData Data => data as FrostySdk.Ebx.HoverLiftPhysicsActionData;
		public override string DisplayName => "HoverLiftPhysicsAction";

		public HoverLiftPhysicsAction(FrostySdk.Ebx.HoverLiftPhysicsActionData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

