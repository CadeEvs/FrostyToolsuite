using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SideToSideManeuverEntityData))]
	public class SideToSideManeuverEntity : DogFightManeuverEntityBase, IEntityData<FrostySdk.Ebx.SideToSideManeuverEntityData>
	{
		public new FrostySdk.Ebx.SideToSideManeuverEntityData Data => data as FrostySdk.Ebx.SideToSideManeuverEntityData;
		public override string DisplayName => "SideToSideManeuver";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SideToSideManeuverEntity(FrostySdk.Ebx.SideToSideManeuverEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

