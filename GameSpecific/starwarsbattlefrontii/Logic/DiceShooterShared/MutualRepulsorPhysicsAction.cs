using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MutualRepulsorPhysicsActionData))]
	public class MutualRepulsorPhysicsAction : PhysicsActionBase, IEntityData<FrostySdk.Ebx.MutualRepulsorPhysicsActionData>
	{
		public new FrostySdk.Ebx.MutualRepulsorPhysicsActionData Data => data as FrostySdk.Ebx.MutualRepulsorPhysicsActionData;
		public override string DisplayName => "MutualRepulsorPhysicsAction";

		public MutualRepulsorPhysicsAction(FrostySdk.Ebx.MutualRepulsorPhysicsActionData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

