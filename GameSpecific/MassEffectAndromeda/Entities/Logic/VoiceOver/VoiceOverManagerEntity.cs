using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VoiceOverManagerEntityData))]
	public class VoiceOverManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.VoiceOverManagerEntityData>
	{
		public new FrostySdk.Ebx.VoiceOverManagerEntityData Data => data as FrostySdk.Ebx.VoiceOverManagerEntityData;
		public override string DisplayName => "VoiceOverManager";

		public VoiceOverManagerEntity(FrostySdk.Ebx.VoiceOverManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

