using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlotLocationMarkerEntityData))]
	public class PlotLocationMarkerEntity : LocationMarkerEntity, IEntityData<FrostySdk.Ebx.PlotLocationMarkerEntityData>
	{
		public new FrostySdk.Ebx.PlotLocationMarkerEntityData Data => data as FrostySdk.Ebx.PlotLocationMarkerEntityData;

		public PlotLocationMarkerEntity(FrostySdk.Ebx.PlotLocationMarkerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

