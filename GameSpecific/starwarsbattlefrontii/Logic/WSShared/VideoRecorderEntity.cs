using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VideoRecorderEntityData))]
	public class VideoRecorderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.VideoRecorderEntityData>
	{
		public new FrostySdk.Ebx.VideoRecorderEntityData Data => data as FrostySdk.Ebx.VideoRecorderEntityData;
		public override string DisplayName => "VideoRecorder";

		public VideoRecorderEntity(FrostySdk.Ebx.VideoRecorderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

