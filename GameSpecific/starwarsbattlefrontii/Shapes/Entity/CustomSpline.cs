using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CustomSplineData))]
	public class CustomSpline : VectorShape, IEntityData<FrostySdk.Ebx.CustomSplineData>
	{
		public new FrostySdk.Ebx.CustomSplineData Data => data as FrostySdk.Ebx.CustomSplineData;
		public override string DisplayName => "CustomSpline";

		public CustomSpline(FrostySdk.Ebx.CustomSplineData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

