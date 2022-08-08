using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpinDescentManeuverEntityData))]
	public class SpinDescentManeuverEntity : DogFightManeuverEntityBase, IEntityData<FrostySdk.Ebx.SpinDescentManeuverEntityData>
	{
		public new FrostySdk.Ebx.SpinDescentManeuverEntityData Data => data as FrostySdk.Ebx.SpinDescentManeuverEntityData;
		public override string DisplayName => "SpinDescentManeuver";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SpinDescentManeuverEntity(FrostySdk.Ebx.SpinDescentManeuverEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

