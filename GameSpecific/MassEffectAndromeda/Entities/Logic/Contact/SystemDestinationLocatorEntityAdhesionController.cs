using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SystemDestinationLocatorEntityAdhesionControllerData))]
	public class SystemDestinationLocatorEntityAdhesionController : LogicController, IEntityData<FrostySdk.Ebx.SystemDestinationLocatorEntityAdhesionControllerData>
	{
		public new FrostySdk.Ebx.SystemDestinationLocatorEntityAdhesionControllerData Data => data as FrostySdk.Ebx.SystemDestinationLocatorEntityAdhesionControllerData;
		public override string DisplayName => "SystemDestinationLocatorEntityAdhesionController";

		public SystemDestinationLocatorEntityAdhesionController(FrostySdk.Ebx.SystemDestinationLocatorEntityAdhesionControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

