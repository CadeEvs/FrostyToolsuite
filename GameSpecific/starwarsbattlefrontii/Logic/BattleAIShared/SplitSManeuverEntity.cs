using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SplitSManeuverEntityData))]
	public class SplitSManeuverEntity : DogFightManeuverEntityBase, IEntityData<FrostySdk.Ebx.SplitSManeuverEntityData>
	{
		public new FrostySdk.Ebx.SplitSManeuverEntityData Data => data as FrostySdk.Ebx.SplitSManeuverEntityData;
		public override string DisplayName => "SplitSManeuver";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SplitSManeuverEntity(FrostySdk.Ebx.SplitSManeuverEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

