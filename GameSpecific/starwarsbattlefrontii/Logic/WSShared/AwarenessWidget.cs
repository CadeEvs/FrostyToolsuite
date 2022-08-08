using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AwarenessWidgetData))]
	public class AwarenessWidget : WSUIWidgetEntity, IEntityData<FrostySdk.Ebx.AwarenessWidgetData>
	{
		public new FrostySdk.Ebx.AwarenessWidgetData Data => data as FrostySdk.Ebx.AwarenessWidgetData;
		public override string DisplayName => "AwarenessWidget";

		public AwarenessWidget(FrostySdk.Ebx.AwarenessWidgetData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

