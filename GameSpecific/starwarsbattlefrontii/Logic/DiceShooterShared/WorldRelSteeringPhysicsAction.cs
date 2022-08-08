using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WorldRelSteeringPhysicsActionData))]
	public class WorldRelSteeringPhysicsAction : PhysicsActionBase, IEntityData<FrostySdk.Ebx.WorldRelSteeringPhysicsActionData>
	{
		public new FrostySdk.Ebx.WorldRelSteeringPhysicsActionData Data => data as FrostySdk.Ebx.WorldRelSteeringPhysicsActionData;
		public override string DisplayName => "WorldRelSteeringPhysicsAction";

		public WorldRelSteeringPhysicsAction(FrostySdk.Ebx.WorldRelSteeringPhysicsActionData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

