using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerUnlockedVoiceLineFilterData))]
	public class PlayerUnlockedVoiceLineFilter : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerUnlockedVoiceLineFilterData>
	{
		public new FrostySdk.Ebx.PlayerUnlockedVoiceLineFilterData Data => data as FrostySdk.Ebx.PlayerUnlockedVoiceLineFilterData;
		public override string DisplayName => "PlayerUnlockedVoiceLineFilter";

		public PlayerUnlockedVoiceLineFilter(FrostySdk.Ebx.PlayerUnlockedVoiceLineFilterData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

