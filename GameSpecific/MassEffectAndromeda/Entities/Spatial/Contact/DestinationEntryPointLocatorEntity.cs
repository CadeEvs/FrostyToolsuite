using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DestinationEntryPointLocatorEntityData))]
	public class DestinationEntryPointLocatorEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.DestinationEntryPointLocatorEntityData>
	{
		public new FrostySdk.Ebx.DestinationEntryPointLocatorEntityData Data => data as FrostySdk.Ebx.DestinationEntryPointLocatorEntityData;

		public DestinationEntryPointLocatorEntity(FrostySdk.Ebx.DestinationEntryPointLocatorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

