using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEScreenshotCaptureEntityData))]
	public class MEScreenshotCaptureEntity : ScreenshotCaptureEntity, IEntityData<FrostySdk.Ebx.MEScreenshotCaptureEntityData>
	{
		public new FrostySdk.Ebx.MEScreenshotCaptureEntityData Data => data as FrostySdk.Ebx.MEScreenshotCaptureEntityData;
		public override string DisplayName => "MEScreenshotCapture";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public MEScreenshotCaptureEntity(FrostySdk.Ebx.MEScreenshotCaptureEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

