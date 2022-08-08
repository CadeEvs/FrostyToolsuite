using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HoverTankPhysicsActionData))]
	public class HoverTankPhysicsAction : PhysicsActionBase, IEntityData<FrostySdk.Ebx.HoverTankPhysicsActionData>
	{
		public new FrostySdk.Ebx.HoverTankPhysicsActionData Data => data as FrostySdk.Ebx.HoverTankPhysicsActionData;
		public override string DisplayName => "HoverTankPhysicsAction";

		public HoverTankPhysicsAction(FrostySdk.Ebx.HoverTankPhysicsActionData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

