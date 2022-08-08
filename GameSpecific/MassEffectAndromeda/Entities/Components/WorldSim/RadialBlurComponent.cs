using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RadialBlurComponentData))]
	public class RadialBlurComponent : VisualEnvironmentComponent, IEntityData<FrostySdk.Ebx.RadialBlurComponentData>
	{
		public new FrostySdk.Ebx.RadialBlurComponentData Data => data as FrostySdk.Ebx.RadialBlurComponentData;
		public override string DisplayName => "RadialBlurComponent";

		public RadialBlurComponent(FrostySdk.Ebx.RadialBlurComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

