using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PhysicsActionData))]
	public class PhysicsAction : LogicEntity, IEntityData<FrostySdk.Ebx.PhysicsActionData>
	{
		public new FrostySdk.Ebx.PhysicsActionData Data => data as FrostySdk.Ebx.PhysicsActionData;
		public override string DisplayName => "PhysicsAction";

		public PhysicsAction(FrostySdk.Ebx.PhysicsActionData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

