using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BWAnimationControlledComponentData))]
	public class BWAnimationControlledComponent : GameComponent, IEntityData<FrostySdk.Ebx.BWAnimationControlledComponentData>
	{
		public new FrostySdk.Ebx.BWAnimationControlledComponentData Data => data as FrostySdk.Ebx.BWAnimationControlledComponentData;
		public override string DisplayName => "BWAnimationControlledComponent";

		public BWAnimationControlledComponent(FrostySdk.Ebx.BWAnimationControlledComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

