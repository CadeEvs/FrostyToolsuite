using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StarFighterPhysicsActionData))]
	public class StarFighterPhysicsAction : PhysicsActionBase, IEntityData<FrostySdk.Ebx.StarFighterPhysicsActionData>
	{
		public new FrostySdk.Ebx.StarFighterPhysicsActionData Data => data as FrostySdk.Ebx.StarFighterPhysicsActionData;
		public override string DisplayName => "StarFighterPhysicsAction";

		public StarFighterPhysicsAction(FrostySdk.Ebx.StarFighterPhysicsActionData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

