using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GalaxyLocatorEntityData))]
	public class GalaxyLocatorEntity : PlotLocationMarkerEntity, IEntityData<FrostySdk.Ebx.GalaxyLocatorEntityData>
	{
		public new FrostySdk.Ebx.GalaxyLocatorEntityData Data => data as FrostySdk.Ebx.GalaxyLocatorEntityData;

		public GalaxyLocatorEntity(FrostySdk.Ebx.GalaxyLocatorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

