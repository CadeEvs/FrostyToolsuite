using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VoiceOverObjectReaderWatcherEntityData))]
	public class VoiceOverObjectReaderWatcherEntity : LogicEntity, IEntityData<FrostySdk.Ebx.VoiceOverObjectReaderWatcherEntityData>
	{
		public new FrostySdk.Ebx.VoiceOverObjectReaderWatcherEntityData Data => data as FrostySdk.Ebx.VoiceOverObjectReaderWatcherEntityData;
		public override string DisplayName => "VoiceOverObjectReaderWatcher";

		public VoiceOverObjectReaderWatcherEntity(FrostySdk.Ebx.VoiceOverObjectReaderWatcherEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

