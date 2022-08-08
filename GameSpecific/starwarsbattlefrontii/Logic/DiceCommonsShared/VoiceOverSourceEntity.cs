using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VoiceOverSourceEntityData))]
	public class VoiceOverSourceEntity : LogicEntity, IEntityData<FrostySdk.Ebx.VoiceOverSourceEntityData>
	{
		public new FrostySdk.Ebx.VoiceOverSourceEntityData Data => data as FrostySdk.Ebx.VoiceOverSourceEntityData;
		public override string DisplayName => "VoiceOverSource";

		public VoiceOverSourceEntity(FrostySdk.Ebx.VoiceOverSourceEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

