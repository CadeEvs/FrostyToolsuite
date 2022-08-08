using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DogFightManeuverEntityBaseData))]
	public class DogFightManeuverEntityBase : LogicEntity, IEntityData<FrostySdk.Ebx.DogFightManeuverEntityBaseData>
	{
		public new FrostySdk.Ebx.DogFightManeuverEntityBaseData Data => data as FrostySdk.Ebx.DogFightManeuverEntityBaseData;
		public override string DisplayName => "DogFightManeuverEntityBase";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DogFightManeuverEntityBase(FrostySdk.Ebx.DogFightManeuverEntityBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

