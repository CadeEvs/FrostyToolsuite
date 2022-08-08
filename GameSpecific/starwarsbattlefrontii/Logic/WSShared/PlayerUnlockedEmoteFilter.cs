using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerUnlockedEmoteFilterData))]
	public class PlayerUnlockedEmoteFilter : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerUnlockedEmoteFilterData>
	{
		public new FrostySdk.Ebx.PlayerUnlockedEmoteFilterData Data => data as FrostySdk.Ebx.PlayerUnlockedEmoteFilterData;
		public override string DisplayName => "PlayerUnlockedEmoteFilter";

		public PlayerUnlockedEmoteFilter(FrostySdk.Ebx.PlayerUnlockedEmoteFilterData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

