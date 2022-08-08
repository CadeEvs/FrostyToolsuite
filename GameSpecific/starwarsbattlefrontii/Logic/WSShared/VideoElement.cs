using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VideoElementData))]
	public class VideoElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.VideoElementData>
	{
		public new FrostySdk.Ebx.VideoElementData Data => data as FrostySdk.Ebx.VideoElementData;
		public override string DisplayName => "VideoElement";

		public VideoElement(FrostySdk.Ebx.VideoElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

