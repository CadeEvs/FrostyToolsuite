using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SystemNavigationForwardControllerData))]
	public class SystemNavigationForwardController : ModifierController, IEntityData<FrostySdk.Ebx.SystemNavigationForwardControllerData>
	{
		public new FrostySdk.Ebx.SystemNavigationForwardControllerData Data => data as FrostySdk.Ebx.SystemNavigationForwardControllerData;
		public override string DisplayName => "SystemNavigationForwardController";

		public SystemNavigationForwardController(FrostySdk.Ebx.SystemNavigationForwardControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

