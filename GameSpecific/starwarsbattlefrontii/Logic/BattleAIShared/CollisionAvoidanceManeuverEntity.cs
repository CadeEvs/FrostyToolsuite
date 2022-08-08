using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CollisionAvoidanceManeuverEntityData))]
	public class CollisionAvoidanceManeuverEntity : DogFightManeuverEntityBase, IEntityData<FrostySdk.Ebx.CollisionAvoidanceManeuverEntityData>
	{
		public new FrostySdk.Ebx.CollisionAvoidanceManeuverEntityData Data => data as FrostySdk.Ebx.CollisionAvoidanceManeuverEntityData;
		public override string DisplayName => "CollisionAvoidanceManeuver";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CollisionAvoidanceManeuverEntity(FrostySdk.Ebx.CollisionAvoidanceManeuverEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

