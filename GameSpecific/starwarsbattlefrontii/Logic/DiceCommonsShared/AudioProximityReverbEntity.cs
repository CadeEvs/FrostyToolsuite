using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AudioProximityReverbEntityData))]
	public class AudioProximityReverbEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AudioProximityReverbEntityData>
	{
		public new FrostySdk.Ebx.AudioProximityReverbEntityData Data => data as FrostySdk.Ebx.AudioProximityReverbEntityData;
		public override string DisplayName => "AudioProximityReverb";

		public AudioProximityReverbEntity(FrostySdk.Ebx.AudioProximityReverbEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

