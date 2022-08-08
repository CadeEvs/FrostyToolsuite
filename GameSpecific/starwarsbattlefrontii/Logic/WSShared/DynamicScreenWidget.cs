using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DynamicScreenWidgetData))]
	public class DynamicScreenWidget : WSUIWidgetEntity, IEntityData<FrostySdk.Ebx.DynamicScreenWidgetData>
	{
		public new FrostySdk.Ebx.DynamicScreenWidgetData Data => data as FrostySdk.Ebx.DynamicScreenWidgetData;
		public override string DisplayName => "DynamicScreenWidget";

		public DynamicScreenWidget(FrostySdk.Ebx.DynamicScreenWidgetData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

