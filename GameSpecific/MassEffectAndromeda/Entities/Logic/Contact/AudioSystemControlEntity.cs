using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AudioSystemControlEntityData))]
	public class AudioSystemControlEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AudioSystemControlEntityData>
	{
		public new FrostySdk.Ebx.AudioSystemControlEntityData Data => data as FrostySdk.Ebx.AudioSystemControlEntityData;
		public override string DisplayName => "AudioSystemControl";

		public AudioSystemControlEntity(FrostySdk.Ebx.AudioSystemControlEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

