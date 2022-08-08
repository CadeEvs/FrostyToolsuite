using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ScreenshotCaptureEntityData))]
	public class ScreenshotCaptureEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ScreenshotCaptureEntityData>
	{
		public new FrostySdk.Ebx.ScreenshotCaptureEntityData Data => data as FrostySdk.Ebx.ScreenshotCaptureEntityData;
		public override string DisplayName => "ScreenshotCapture";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ScreenshotCaptureEntity(FrostySdk.Ebx.ScreenshotCaptureEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

