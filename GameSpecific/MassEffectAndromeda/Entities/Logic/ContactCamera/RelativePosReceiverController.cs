using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RelativePosReceiverControllerData))]
	public class RelativePosReceiverController : ValueModifierController, IEntityData<FrostySdk.Ebx.RelativePosReceiverControllerData>
	{
		public new FrostySdk.Ebx.RelativePosReceiverControllerData Data => data as FrostySdk.Ebx.RelativePosReceiverControllerData;
		public override string DisplayName => "RelativePosReceiverController";

		public RelativePosReceiverController(FrostySdk.Ebx.RelativePosReceiverControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

