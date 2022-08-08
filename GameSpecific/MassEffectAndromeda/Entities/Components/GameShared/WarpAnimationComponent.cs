using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WarpAnimationComponentData))]
	public class WarpAnimationComponent : GameComponent, IEntityData<FrostySdk.Ebx.WarpAnimationComponentData>
	{
		public new FrostySdk.Ebx.WarpAnimationComponentData Data => data as FrostySdk.Ebx.WarpAnimationComponentData;
		public override string DisplayName => "WarpAnimationComponent";

		public WarpAnimationComponent(FrostySdk.Ebx.WarpAnimationComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

