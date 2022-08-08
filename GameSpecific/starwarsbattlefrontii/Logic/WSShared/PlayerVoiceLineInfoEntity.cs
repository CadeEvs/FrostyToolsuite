using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerVoiceLineInfoEntityData))]
	public class PlayerVoiceLineInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerVoiceLineInfoEntityData>
	{
		public new FrostySdk.Ebx.PlayerVoiceLineInfoEntityData Data => data as FrostySdk.Ebx.PlayerVoiceLineInfoEntityData;
		public override string DisplayName => "PlayerVoiceLineInfo";

		public PlayerVoiceLineInfoEntity(FrostySdk.Ebx.PlayerVoiceLineInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

