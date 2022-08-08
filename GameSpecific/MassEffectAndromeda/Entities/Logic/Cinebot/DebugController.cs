using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DebugControllerData))]
	public class DebugController : BasicController, IEntityData<FrostySdk.Ebx.DebugControllerData>
	{
		public new FrostySdk.Ebx.DebugControllerData Data => data as FrostySdk.Ebx.DebugControllerData;
		public override string DisplayName => "DebugController";

		public DebugController(FrostySdk.Ebx.DebugControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

