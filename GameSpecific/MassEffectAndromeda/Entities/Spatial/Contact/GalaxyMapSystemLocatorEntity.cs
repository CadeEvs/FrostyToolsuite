using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GalaxyMapSystemLocatorEntityData))]
	public class GalaxyMapSystemLocatorEntity : GalaxyLocatorEntity, IEntityData<FrostySdk.Ebx.GalaxyMapSystemLocatorEntityData>
	{
		public new FrostySdk.Ebx.GalaxyMapSystemLocatorEntityData Data => data as FrostySdk.Ebx.GalaxyMapSystemLocatorEntityData;

		public GalaxyMapSystemLocatorEntity(FrostySdk.Ebx.GalaxyMapSystemLocatorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

