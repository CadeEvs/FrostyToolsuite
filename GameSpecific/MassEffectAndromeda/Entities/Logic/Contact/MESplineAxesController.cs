using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MESplineAxesControllerData))]
	public class MESplineAxesController : SplineAxesController, IEntityData<FrostySdk.Ebx.MESplineAxesControllerData>
	{
		public new FrostySdk.Ebx.MESplineAxesControllerData Data => data as FrostySdk.Ebx.MESplineAxesControllerData;
		public override string DisplayName => "MESplineAxesController";

		public MESplineAxesController(FrostySdk.Ebx.MESplineAxesControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

