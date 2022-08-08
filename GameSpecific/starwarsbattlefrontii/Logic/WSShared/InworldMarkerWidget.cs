using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.InworldMarkerWidgetData))]
	public class InworldMarkerWidget : WSUIWidgetEntity, IEntityData<FrostySdk.Ebx.InworldMarkerWidgetData>
	{
		public new FrostySdk.Ebx.InworldMarkerWidgetData Data => data as FrostySdk.Ebx.InworldMarkerWidgetData;
		public override string DisplayName => "InworldMarkerWidget";

		public InworldMarkerWidget(FrostySdk.Ebx.InworldMarkerWidgetData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

