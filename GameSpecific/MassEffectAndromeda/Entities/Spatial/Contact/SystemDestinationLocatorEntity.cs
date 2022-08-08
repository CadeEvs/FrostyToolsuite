using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SystemDestinationLocatorEntityData))]
	public class SystemDestinationLocatorEntity : GalaxyLocatorEntity, IEntityData<FrostySdk.Ebx.SystemDestinationLocatorEntityData>
	{
		public new FrostySdk.Ebx.SystemDestinationLocatorEntityData Data => data as FrostySdk.Ebx.SystemDestinationLocatorEntityData;

		public SystemDestinationLocatorEntity(FrostySdk.Ebx.SystemDestinationLocatorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

