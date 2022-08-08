using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BarrelRollManeuverEntityData))]
	public class BarrelRollManeuverEntity : DogFightManeuverEntityBase, IEntityData<FrostySdk.Ebx.BarrelRollManeuverEntityData>
	{
		public new FrostySdk.Ebx.BarrelRollManeuverEntityData Data => data as FrostySdk.Ebx.BarrelRollManeuverEntityData;
		public override string DisplayName => "BarrelRollManeuver";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public BarrelRollManeuverEntity(FrostySdk.Ebx.BarrelRollManeuverEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

