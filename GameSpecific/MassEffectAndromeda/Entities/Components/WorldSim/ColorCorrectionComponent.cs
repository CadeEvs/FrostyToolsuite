using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ColorCorrectionComponentData))]
	public class ColorCorrectionComponent : VisualEnvironmentComponent, IEntityData<FrostySdk.Ebx.ColorCorrectionComponentData>
	{
		public new FrostySdk.Ebx.ColorCorrectionComponentData Data => data as FrostySdk.Ebx.ColorCorrectionComponentData;
		public override string DisplayName => "ColorCorrectionComponent";

		public ColorCorrectionComponent(FrostySdk.Ebx.ColorCorrectionComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

