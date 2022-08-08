using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SamplerSplineData))]
	public class SamplerSpline : CustomSpline, IEntityData<FrostySdk.Ebx.SamplerSplineData>
	{
		public new FrostySdk.Ebx.SamplerSplineData Data => data as FrostySdk.Ebx.SamplerSplineData;
		public override string DisplayName => "SamplerSpline";

		public SamplerSpline(FrostySdk.Ebx.SamplerSplineData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

