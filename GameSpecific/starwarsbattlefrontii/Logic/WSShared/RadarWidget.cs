using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RadarWidgetData))]
	public class RadarWidget : WSUIWidgetEntity, IEntityData<FrostySdk.Ebx.RadarWidgetData>
	{
		public new FrostySdk.Ebx.RadarWidgetData Data => data as FrostySdk.Ebx.RadarWidgetData;
		public override string DisplayName => "RadarWidget";

		public RadarWidget(FrostySdk.Ebx.RadarWidgetData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

