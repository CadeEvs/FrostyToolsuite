using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ExtendedControllerData))]
	public class ExtendedController : CinebotController, IEntityData<FrostySdk.Ebx.ExtendedControllerData>
	{
		public new FrostySdk.Ebx.ExtendedControllerData Data => data as FrostySdk.Ebx.ExtendedControllerData;
		public override string DisplayName => "ExtendedController";

		public ExtendedController(FrostySdk.Ebx.ExtendedControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

