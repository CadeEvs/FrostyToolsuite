using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CreateDistanceManeuverEntityData))]
	public class CreateDistanceManeuverEntity : DogFightManeuverEntityBase, IEntityData<FrostySdk.Ebx.CreateDistanceManeuverEntityData>
	{
		public new FrostySdk.Ebx.CreateDistanceManeuverEntityData Data => data as FrostySdk.Ebx.CreateDistanceManeuverEntityData;
		public override string DisplayName => "CreateDistanceManeuver";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CreateDistanceManeuverEntity(FrostySdk.Ebx.CreateDistanceManeuverEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

