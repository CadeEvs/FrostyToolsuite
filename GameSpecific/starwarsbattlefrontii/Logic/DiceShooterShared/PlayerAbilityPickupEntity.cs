using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerAbilityPickupEntityData))]
	public class PlayerAbilityPickupEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerAbilityPickupEntityData>
	{
		public new FrostySdk.Ebx.PlayerAbilityPickupEntityData Data => data as FrostySdk.Ebx.PlayerAbilityPickupEntityData;
		public override string DisplayName => "PlayerAbilityPickup";

		public PlayerAbilityPickupEntity(FrostySdk.Ebx.PlayerAbilityPickupEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

