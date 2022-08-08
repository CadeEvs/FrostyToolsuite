using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BasicDefensiveManeuverEntityData))]
	public class BasicDefensiveManeuverEntity : DogFightManeuverEntityBase, IEntityData<FrostySdk.Ebx.BasicDefensiveManeuverEntityData>
	{
		public new FrostySdk.Ebx.BasicDefensiveManeuverEntityData Data => data as FrostySdk.Ebx.BasicDefensiveManeuverEntityData;
		public override string DisplayName => "BasicDefensiveManeuver";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public BasicDefensiveManeuverEntity(FrostySdk.Ebx.BasicDefensiveManeuverEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

