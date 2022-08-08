using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FlyToManeuverEntityData))]
	public class FlyToManeuverEntity : DogFightManeuverEntityBase, IEntityData<FrostySdk.Ebx.FlyToManeuverEntityData>
	{
		public new FrostySdk.Ebx.FlyToManeuverEntityData Data => data as FrostySdk.Ebx.FlyToManeuverEntityData;
		public override string DisplayName => "FlyToManeuver";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public FlyToManeuverEntity(FrostySdk.Ebx.FlyToManeuverEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

