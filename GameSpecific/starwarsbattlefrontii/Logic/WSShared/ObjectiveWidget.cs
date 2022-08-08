using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ObjectiveWidgetData))]
	public class ObjectiveWidget : WSUIWidgetEntity, IEntityData<FrostySdk.Ebx.ObjectiveWidgetData>
	{
		public new FrostySdk.Ebx.ObjectiveWidgetData Data => data as FrostySdk.Ebx.ObjectiveWidgetData;
		public override string DisplayName => "ObjectiveWidget";

		public ObjectiveWidget(FrostySdk.Ebx.ObjectiveWidgetData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

