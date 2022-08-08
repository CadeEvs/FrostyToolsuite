using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ShakeControllerData))]
	public class ShakeController : ModifierController, IEntityData<FrostySdk.Ebx.ShakeControllerData>
	{
		public new FrostySdk.Ebx.ShakeControllerData Data => data as FrostySdk.Ebx.ShakeControllerData;
		public override string DisplayName => "ShakeController";

		public ShakeController(FrostySdk.Ebx.ShakeControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

