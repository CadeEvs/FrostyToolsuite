using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RollOnInputPhysicsActionData))]
	public class RollOnInputPhysicsAction : PhysicsActionBase, IEntityData<FrostySdk.Ebx.RollOnInputPhysicsActionData>
	{
		public new FrostySdk.Ebx.RollOnInputPhysicsActionData Data => data as FrostySdk.Ebx.RollOnInputPhysicsActionData;
		public override string DisplayName => "RollOnInputPhysicsAction";

		public RollOnInputPhysicsAction(FrostySdk.Ebx.RollOnInputPhysicsActionData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

