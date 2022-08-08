using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BWAnimationComponentData))]
	public class BWAnimationComponent : GameComponent, IEntityData<FrostySdk.Ebx.BWAnimationComponentData>
	{
		public new FrostySdk.Ebx.BWAnimationComponentData Data => data as FrostySdk.Ebx.BWAnimationComponentData;
		public override string DisplayName => "BWAnimationComponent";

		public BWAnimationComponent(FrostySdk.Ebx.BWAnimationComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

