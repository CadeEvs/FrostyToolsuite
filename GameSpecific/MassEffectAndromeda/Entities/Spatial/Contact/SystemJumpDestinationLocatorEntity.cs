using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SystemJumpDestinationLocatorEntityData))]
	public class SystemJumpDestinationLocatorEntity : SystemDestinationLocatorEntity, IEntityData<FrostySdk.Ebx.SystemJumpDestinationLocatorEntityData>
	{
		public new FrostySdk.Ebx.SystemJumpDestinationLocatorEntityData Data => data as FrostySdk.Ebx.SystemJumpDestinationLocatorEntityData;

		public SystemJumpDestinationLocatorEntity(FrostySdk.Ebx.SystemJumpDestinationLocatorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

