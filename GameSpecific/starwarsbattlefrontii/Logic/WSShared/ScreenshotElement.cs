using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ScreenshotElementData))]
	public class ScreenshotElement : WSUISoloBatchableElement, IEntityData<FrostySdk.Ebx.ScreenshotElementData>
	{
		public new FrostySdk.Ebx.ScreenshotElementData Data => data as FrostySdk.Ebx.ScreenshotElementData;
		public override string DisplayName => "ScreenshotElement";

		public ScreenshotElement(FrostySdk.Ebx.ScreenshotElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

