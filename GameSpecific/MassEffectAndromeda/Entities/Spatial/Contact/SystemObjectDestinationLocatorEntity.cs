using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SystemObjectDestinationLocatorEntityData))]
	public class SystemObjectDestinationLocatorEntity : SystemDestinationLocatorEntity, IEntityData<FrostySdk.Ebx.SystemObjectDestinationLocatorEntityData>
	{
		public new FrostySdk.Ebx.SystemObjectDestinationLocatorEntityData Data => data as FrostySdk.Ebx.SystemObjectDestinationLocatorEntityData;

		public SystemObjectDestinationLocatorEntity(FrostySdk.Ebx.SystemObjectDestinationLocatorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

