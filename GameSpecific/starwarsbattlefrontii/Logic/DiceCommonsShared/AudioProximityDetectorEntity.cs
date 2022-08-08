using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AudioProximityDetectorEntityData))]
	public class AudioProximityDetectorEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AudioProximityDetectorEntityData>
	{
		public new FrostySdk.Ebx.AudioProximityDetectorEntityData Data => data as FrostySdk.Ebx.AudioProximityDetectorEntityData;
		public override string DisplayName => "AudioProximityDetector";

		public AudioProximityDetectorEntity(FrostySdk.Ebx.AudioProximityDetectorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

