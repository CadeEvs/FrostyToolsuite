using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.InputControllerData))]
	public class InputController : LogicController, IEntityData<FrostySdk.Ebx.InputControllerData>
	{
		public new FrostySdk.Ebx.InputControllerData Data => data as FrostySdk.Ebx.InputControllerData;
		public override string DisplayName => "InputController";

		public InputController(FrostySdk.Ebx.InputControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

