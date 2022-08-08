using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DebugOffsetControllerData))]
	public class DebugOffsetController : OffsetController, IEntityData<FrostySdk.Ebx.DebugOffsetControllerData>
	{
		public new FrostySdk.Ebx.DebugOffsetControllerData Data => data as FrostySdk.Ebx.DebugOffsetControllerData;
		public override string DisplayName => "DebugOffsetController";

		public DebugOffsetController(FrostySdk.Ebx.DebugOffsetControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

