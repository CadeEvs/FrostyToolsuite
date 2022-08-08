using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SystemEntryPointLocatorEntityData))]
	public class SystemEntryPointLocatorEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.SystemEntryPointLocatorEntityData>
	{
		public new FrostySdk.Ebx.SystemEntryPointLocatorEntityData Data => data as FrostySdk.Ebx.SystemEntryPointLocatorEntityData;

		public SystemEntryPointLocatorEntity(FrostySdk.Ebx.SystemEntryPointLocatorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

