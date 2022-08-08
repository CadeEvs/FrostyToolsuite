using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VoiceOverObjectReaderEntityData))]
	public class VoiceOverObjectReaderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.VoiceOverObjectReaderEntityData>
	{
		public new FrostySdk.Ebx.VoiceOverObjectReaderEntityData Data => data as FrostySdk.Ebx.VoiceOverObjectReaderEntityData;
		public override string DisplayName => "VoiceOverObjectReader";

		public VoiceOverObjectReaderEntity(FrostySdk.Ebx.VoiceOverObjectReaderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

