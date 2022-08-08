using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RadarWidgetCollectorElementData))]
	public class RadarWidgetCollectorElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.RadarWidgetCollectorElementData>
	{
		public new FrostySdk.Ebx.RadarWidgetCollectorElementData Data => data as FrostySdk.Ebx.RadarWidgetCollectorElementData;
		public override string DisplayName => "RadarWidgetCollectorElement";

		public RadarWidgetCollectorElement(FrostySdk.Ebx.RadarWidgetCollectorElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

