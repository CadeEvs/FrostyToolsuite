using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DefensiveManeuverSelectorEntityData))]
	public class DefensiveManeuverSelectorEntity : ManeuverSelectorEntity, IEntityData<FrostySdk.Ebx.DefensiveManeuverSelectorEntityData>
	{
		public new FrostySdk.Ebx.DefensiveManeuverSelectorEntityData Data => data as FrostySdk.Ebx.DefensiveManeuverSelectorEntityData;
		public override string DisplayName => "DefensiveManeuverSelector";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DefensiveManeuverSelectorEntity(FrostySdk.Ebx.DefensiveManeuverSelectorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

