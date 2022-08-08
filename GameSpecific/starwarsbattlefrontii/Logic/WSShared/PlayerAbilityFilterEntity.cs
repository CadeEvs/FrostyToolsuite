using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerAbilityFilterEntityData))]
	public class PlayerAbilityFilterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerAbilityFilterEntityData>
	{
		public new FrostySdk.Ebx.PlayerAbilityFilterEntityData Data => data as FrostySdk.Ebx.PlayerAbilityFilterEntityData;
		public override string DisplayName => "PlayerAbilityFilter";

		public PlayerAbilityFilterEntity(FrostySdk.Ebx.PlayerAbilityFilterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

