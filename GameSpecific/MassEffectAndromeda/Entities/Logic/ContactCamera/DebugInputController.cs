using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DebugInputControllerData))]
	public class DebugInputController : LogicController, IEntityData<FrostySdk.Ebx.DebugInputControllerData>
	{
		public new FrostySdk.Ebx.DebugInputControllerData Data => data as FrostySdk.Ebx.DebugInputControllerData;
		public override string DisplayName => "DebugInputController";

		public DebugInputController(FrostySdk.Ebx.DebugInputControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

