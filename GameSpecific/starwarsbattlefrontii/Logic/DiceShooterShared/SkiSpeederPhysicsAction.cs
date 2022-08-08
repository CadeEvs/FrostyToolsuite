using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SkiSpeederPhysicsActionData))]
	public class SkiSpeederPhysicsAction : PhysicsActionBase, IEntityData<FrostySdk.Ebx.SkiSpeederPhysicsActionData>
	{
		public new FrostySdk.Ebx.SkiSpeederPhysicsActionData Data => data as FrostySdk.Ebx.SkiSpeederPhysicsActionData;
		public override string DisplayName => "SkiSpeederPhysicsAction";

		public SkiSpeederPhysicsAction(FrostySdk.Ebx.SkiSpeederPhysicsActionData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

