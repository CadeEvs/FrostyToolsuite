using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OffsetControllerData))]
	public class OffsetController : ModifierController, IEntityData<FrostySdk.Ebx.OffsetControllerData>
	{
		public new FrostySdk.Ebx.OffsetControllerData Data => data as FrostySdk.Ebx.OffsetControllerData;
		public override string DisplayName => "OffsetController";

		public OffsetController(FrostySdk.Ebx.OffsetControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

