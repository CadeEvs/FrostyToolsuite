using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SplineAxesControllerData))]
	public class SplineAxesController : ModifierController, IEntityData<FrostySdk.Ebx.SplineAxesControllerData>
	{
		public new FrostySdk.Ebx.SplineAxesControllerData Data => data as FrostySdk.Ebx.SplineAxesControllerData;
		public override string DisplayName => "SplineAxesController";

		public SplineAxesController(FrostySdk.Ebx.SplineAxesControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

