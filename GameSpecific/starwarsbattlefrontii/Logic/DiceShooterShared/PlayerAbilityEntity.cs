using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerAbilityEntityData))]
	public class PlayerAbilityEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerAbilityEntityData>
	{
		public new FrostySdk.Ebx.PlayerAbilityEntityData Data => data as FrostySdk.Ebx.PlayerAbilityEntityData;
		public override string DisplayName => "PlayerAbility";

		public PlayerAbilityEntity(FrostySdk.Ebx.PlayerAbilityEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

