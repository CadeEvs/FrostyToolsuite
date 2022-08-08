using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ScreenshotComparisonEntityData))]
	public class ScreenshotComparisonEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ScreenshotComparisonEntityData>
	{
		public new FrostySdk.Ebx.ScreenshotComparisonEntityData Data => data as FrostySdk.Ebx.ScreenshotComparisonEntityData;
		public override string DisplayName => "ScreenshotComparison";

		public ScreenshotComparisonEntity(FrostySdk.Ebx.ScreenshotComparisonEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

