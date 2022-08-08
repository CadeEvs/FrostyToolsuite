using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerVoiceManagerEntityData))]
	public class PlayerVoiceManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerVoiceManagerEntityData>
	{
		public new FrostySdk.Ebx.PlayerVoiceManagerEntityData Data => data as FrostySdk.Ebx.PlayerVoiceManagerEntityData;
		public override string DisplayName => "PlayerVoiceManager";

		public PlayerVoiceManagerEntity(FrostySdk.Ebx.PlayerVoiceManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

