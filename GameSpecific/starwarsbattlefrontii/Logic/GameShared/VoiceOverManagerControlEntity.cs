using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VoiceOverManagerControlEntityData))]
	public class VoiceOverManagerControlEntity : LogicEntity, IEntityData<FrostySdk.Ebx.VoiceOverManagerControlEntityData>
	{
		public new FrostySdk.Ebx.VoiceOverManagerControlEntityData Data => data as FrostySdk.Ebx.VoiceOverManagerControlEntityData;
		public override string DisplayName => "VoiceOverManagerControl";

		public VoiceOverManagerControlEntity(FrostySdk.Ebx.VoiceOverManagerControlEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

