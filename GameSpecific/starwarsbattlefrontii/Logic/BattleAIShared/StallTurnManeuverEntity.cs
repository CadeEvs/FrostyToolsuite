using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StallTurnManeuverEntityData))]
	public class StallTurnManeuverEntity : DogFightManeuverEntityBase, IEntityData<FrostySdk.Ebx.StallTurnManeuverEntityData>
	{
		public new FrostySdk.Ebx.StallTurnManeuverEntityData Data => data as FrostySdk.Ebx.StallTurnManeuverEntityData;
		public override string DisplayName => "StallTurnManeuver";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public StallTurnManeuverEntity(FrostySdk.Ebx.StallTurnManeuverEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

