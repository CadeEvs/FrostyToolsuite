using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VoipPushToTalkEntityData))]
	public class VoipPushToTalkEntity : LogicEntity, IEntityData<FrostySdk.Ebx.VoipPushToTalkEntityData>
	{
		public new FrostySdk.Ebx.VoipPushToTalkEntityData Data => data as FrostySdk.Ebx.VoipPushToTalkEntityData;
		public override string DisplayName => "VoipPushToTalk";

		public VoipPushToTalkEntity(FrostySdk.Ebx.VoipPushToTalkEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

