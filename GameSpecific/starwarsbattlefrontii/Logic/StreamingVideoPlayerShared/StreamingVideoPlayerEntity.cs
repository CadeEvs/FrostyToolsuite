using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StreamingVideoPlayerEntityData))]
	public class StreamingVideoPlayerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.StreamingVideoPlayerEntityData>
	{
		public new FrostySdk.Ebx.StreamingVideoPlayerEntityData Data => data as FrostySdk.Ebx.StreamingVideoPlayerEntityData;
		public override string DisplayName => "StreamingVideoPlayer";

		public StreamingVideoPlayerEntity(FrostySdk.Ebx.StreamingVideoPlayerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

