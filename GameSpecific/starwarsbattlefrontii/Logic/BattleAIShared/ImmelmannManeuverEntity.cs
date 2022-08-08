using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ImmelmannManeuverEntityData))]
	public class ImmelmannManeuverEntity : DogFightManeuverEntityBase, IEntityData<FrostySdk.Ebx.ImmelmannManeuverEntityData>
	{
		public new FrostySdk.Ebx.ImmelmannManeuverEntityData Data => data as FrostySdk.Ebx.ImmelmannManeuverEntityData;
		public override string DisplayName => "ImmelmannManeuver";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ImmelmannManeuverEntity(FrostySdk.Ebx.ImmelmannManeuverEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

