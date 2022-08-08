using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SunFlareComponentData))]
	public class SunFlareComponent : VisualEnvironmentComponent, IEntityData<FrostySdk.Ebx.SunFlareComponentData>
	{
		public new FrostySdk.Ebx.SunFlareComponentData Data => data as FrostySdk.Ebx.SunFlareComponentData;
		public override string DisplayName => "SunFlareComponent";

		public SunFlareComponent(FrostySdk.Ebx.SunFlareComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

