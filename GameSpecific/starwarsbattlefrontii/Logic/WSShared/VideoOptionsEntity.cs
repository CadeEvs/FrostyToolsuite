using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VideoOptionsEntityData))]
	public class VideoOptionsEntity : LogicEntity, IEntityData<FrostySdk.Ebx.VideoOptionsEntityData>
	{
		public new FrostySdk.Ebx.VideoOptionsEntityData Data => data as FrostySdk.Ebx.VideoOptionsEntityData;
		public override string DisplayName => "VideoOptions";

		public VideoOptionsEntity(FrostySdk.Ebx.VideoOptionsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

