using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerUnlockedAbilityFilterData))]
	public class PlayerUnlockedAbilityFilter : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerUnlockedAbilityFilterData>
	{
		public new FrostySdk.Ebx.PlayerUnlockedAbilityFilterData Data => data as FrostySdk.Ebx.PlayerUnlockedAbilityFilterData;
		public override string DisplayName => "PlayerUnlockedAbilityFilter";

		public PlayerUnlockedAbilityFilter(FrostySdk.Ebx.PlayerUnlockedAbilityFilterData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

