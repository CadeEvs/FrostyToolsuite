using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DebugFreeControllerData))]
	public class DebugFreeController : FreeController, IEntityData<FrostySdk.Ebx.DebugFreeControllerData>
	{
		public new FrostySdk.Ebx.DebugFreeControllerData Data => data as FrostySdk.Ebx.DebugFreeControllerData;
		public override string DisplayName => "DebugFreeController";

		public DebugFreeController(FrostySdk.Ebx.DebugFreeControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

