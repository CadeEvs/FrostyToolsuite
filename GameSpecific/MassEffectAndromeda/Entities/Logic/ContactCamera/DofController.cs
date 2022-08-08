using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DofControllerData))]
	public class DofController : ModifierController, IEntityData<FrostySdk.Ebx.DofControllerData>
	{
		public new FrostySdk.Ebx.DofControllerData Data => data as FrostySdk.Ebx.DofControllerData;
		public override string DisplayName => "DofController";

		public DofController(FrostySdk.Ebx.DofControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

