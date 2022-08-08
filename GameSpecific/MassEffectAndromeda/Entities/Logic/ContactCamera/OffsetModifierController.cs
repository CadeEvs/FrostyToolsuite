using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OffsetModifierControllerData))]
	public class OffsetModifierController : ValueModifierController, IEntityData<FrostySdk.Ebx.OffsetModifierControllerData>
	{
		public new FrostySdk.Ebx.OffsetModifierControllerData Data => data as FrostySdk.Ebx.OffsetModifierControllerData;
		public override string DisplayName => "OffsetModifierController";

		public OffsetModifierController(FrostySdk.Ebx.OffsetModifierControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

