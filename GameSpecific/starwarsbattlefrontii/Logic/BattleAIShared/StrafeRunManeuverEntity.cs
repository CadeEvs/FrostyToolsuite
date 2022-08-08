using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StrafeRunManeuverEntityData))]
	public class StrafeRunManeuverEntity : DogFightManeuverEntityBase, IEntityData<FrostySdk.Ebx.StrafeRunManeuverEntityData>
	{
		public new FrostySdk.Ebx.StrafeRunManeuverEntityData Data => data as FrostySdk.Ebx.StrafeRunManeuverEntityData;
		public override string DisplayName => "StrafeRunManeuver";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public StrafeRunManeuverEntity(FrostySdk.Ebx.StrafeRunManeuverEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

