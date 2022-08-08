using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterInVehicleScenarioEntityData))]
	public class CharacterInVehicleScenarioEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CharacterInVehicleScenarioEntityData>
	{
		public new FrostySdk.Ebx.CharacterInVehicleScenarioEntityData Data => data as FrostySdk.Ebx.CharacterInVehicleScenarioEntityData;
		public override string DisplayName => "CharacterInVehicleScenario";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CharacterInVehicleScenarioEntity(FrostySdk.Ebx.CharacterInVehicleScenarioEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

