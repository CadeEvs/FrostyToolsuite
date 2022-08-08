using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LoadLevelWidgetData))]
	public class LoadLevelWidget : WSUIWidgetEntity, IEntityData<FrostySdk.Ebx.LoadLevelWidgetData>
	{
		public new FrostySdk.Ebx.LoadLevelWidgetData Data => data as FrostySdk.Ebx.LoadLevelWidgetData;
		public override string DisplayName => "LoadLevelWidget";

		public LoadLevelWidget(FrostySdk.Ebx.LoadLevelWidgetData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

