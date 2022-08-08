using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LogicControllerData))]
	public class LogicController : ExtendedController, IEntityData<FrostySdk.Ebx.LogicControllerData>
	{
		public new FrostySdk.Ebx.LogicControllerData Data => data as FrostySdk.Ebx.LogicControllerData;
		public override string DisplayName => "LogicController";

		public LogicController(FrostySdk.Ebx.LogicControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

