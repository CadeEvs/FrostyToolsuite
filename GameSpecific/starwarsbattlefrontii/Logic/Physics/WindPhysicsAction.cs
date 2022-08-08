using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WindPhysicsActionData))]
	public class WindPhysicsAction : PhysicsAction, IEntityData<FrostySdk.Ebx.WindPhysicsActionData>
	{
		public new FrostySdk.Ebx.WindPhysicsActionData Data => data as FrostySdk.Ebx.WindPhysicsActionData;
		public override string DisplayName => "WindPhysicsAction";

		public WindPhysicsAction(FrostySdk.Ebx.WindPhysicsActionData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

