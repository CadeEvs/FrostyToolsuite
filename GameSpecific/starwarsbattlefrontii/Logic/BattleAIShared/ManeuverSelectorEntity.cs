using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ManeuverSelectorEntityData))]
	public class ManeuverSelectorEntity : DogFightManeuverEntityBase, IEntityData<FrostySdk.Ebx.ManeuverSelectorEntityData>
	{
		public new FrostySdk.Ebx.ManeuverSelectorEntityData Data => data as FrostySdk.Ebx.ManeuverSelectorEntityData;
		public override string DisplayName => "ManeuverSelector";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ManeuverSelectorEntity(FrostySdk.Ebx.ManeuverSelectorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

