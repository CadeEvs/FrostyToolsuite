using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FloatPhysicsActionData))]
	public class FloatPhysicsAction : PhysicsAction, IEntityData<FrostySdk.Ebx.FloatPhysicsActionData>
	{
		public new FrostySdk.Ebx.FloatPhysicsActionData Data => data as FrostySdk.Ebx.FloatPhysicsActionData;
		public override string DisplayName => "FloatPhysicsAction";

		public FloatPhysicsAction(FrostySdk.Ebx.FloatPhysicsActionData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

