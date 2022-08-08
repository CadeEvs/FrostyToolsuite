using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EnforceAltitudeManeuverEntityData))]
	public class EnforceAltitudeManeuverEntity : DogFightManeuverEntityBase, IEntityData<FrostySdk.Ebx.EnforceAltitudeManeuverEntityData>
	{
		public new FrostySdk.Ebx.EnforceAltitudeManeuverEntityData Data => data as FrostySdk.Ebx.EnforceAltitudeManeuverEntityData;
		public override string DisplayName => "EnforceAltitudeManeuver";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public EnforceAltitudeManeuverEntity(FrostySdk.Ebx.EnforceAltitudeManeuverEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

