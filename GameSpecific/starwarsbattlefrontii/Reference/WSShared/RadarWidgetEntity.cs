using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RadarWidgetEntityData))]
	public class RadarWidgetEntity : LogicReferenceObject, IEntityData<FrostySdk.Ebx.RadarWidgetEntityData>
	{
		public new FrostySdk.Ebx.RadarWidgetEntityData Data => data as FrostySdk.Ebx.RadarWidgetEntityData;

		public RadarWidgetEntity(FrostySdk.Ebx.RadarWidgetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

