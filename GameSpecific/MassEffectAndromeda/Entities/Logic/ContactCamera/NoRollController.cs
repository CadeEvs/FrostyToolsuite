using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.NoRollControllerData))]
	public class NoRollController : ModifierController, IEntityData<FrostySdk.Ebx.NoRollControllerData>
	{
		public new FrostySdk.Ebx.NoRollControllerData Data => data as FrostySdk.Ebx.NoRollControllerData;
		public override string DisplayName => "NoRollController";

		public NoRollController(FrostySdk.Ebx.NoRollControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

