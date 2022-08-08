using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StarfighterSpinoutPhysicsActionData))]
	public class StarfighterSpinoutPhysicsAction : PhysicsActionBase, IEntityData<FrostySdk.Ebx.StarfighterSpinoutPhysicsActionData>
	{
		public new FrostySdk.Ebx.StarfighterSpinoutPhysicsActionData Data => data as FrostySdk.Ebx.StarfighterSpinoutPhysicsActionData;
		public override string DisplayName => "StarfighterSpinoutPhysicsAction";

		public StarfighterSpinoutPhysicsAction(FrostySdk.Ebx.StarfighterSpinoutPhysicsActionData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

