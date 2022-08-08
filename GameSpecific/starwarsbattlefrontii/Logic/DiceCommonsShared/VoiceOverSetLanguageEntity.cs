using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VoiceOverSetLanguageEntityData))]
	public class VoiceOverSetLanguageEntity : LogicEntity, IEntityData<FrostySdk.Ebx.VoiceOverSetLanguageEntityData>
	{
		public new FrostySdk.Ebx.VoiceOverSetLanguageEntityData Data => data as FrostySdk.Ebx.VoiceOverSetLanguageEntityData;
		public override string DisplayName => "VoiceOverSetLanguage";

		public VoiceOverSetLanguageEntity(FrostySdk.Ebx.VoiceOverSetLanguageEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

