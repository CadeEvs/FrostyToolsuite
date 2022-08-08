using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ScreenSpaceLensFlareComponentData))]
	public class ScreenSpaceLensFlareComponent : VisualEnvironmentComponent, IEntityData<FrostySdk.Ebx.ScreenSpaceLensFlareComponentData>
	{
		public new FrostySdk.Ebx.ScreenSpaceLensFlareComponentData Data => data as FrostySdk.Ebx.ScreenSpaceLensFlareComponentData;
		public override string DisplayName => "ScreenSpaceLensFlareComponent";

		public ScreenSpaceLensFlareComponent(FrostySdk.Ebx.ScreenSpaceLensFlareComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

