using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HoverNoisePhysicsActionData))]
	public class HoverNoisePhysicsAction : PhysicsActionBase, IEntityData<FrostySdk.Ebx.HoverNoisePhysicsActionData>
	{
		public new FrostySdk.Ebx.HoverNoisePhysicsActionData Data => data as FrostySdk.Ebx.HoverNoisePhysicsActionData;
		public override string DisplayName => "HoverNoisePhysicsAction";

		public HoverNoisePhysicsAction(FrostySdk.Ebx.HoverNoisePhysicsActionData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

