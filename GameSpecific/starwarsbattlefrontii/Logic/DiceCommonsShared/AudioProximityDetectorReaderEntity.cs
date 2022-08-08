using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AudioProximityDetectorReaderEntityData))]
	public class AudioProximityDetectorReaderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AudioProximityDetectorReaderEntityData>
	{
		public new FrostySdk.Ebx.AudioProximityDetectorReaderEntityData Data => data as FrostySdk.Ebx.AudioProximityDetectorReaderEntityData;
		public override string DisplayName => "AudioProximityDetectorReader";

		public AudioProximityDetectorReaderEntity(FrostySdk.Ebx.AudioProximityDetectorReaderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

