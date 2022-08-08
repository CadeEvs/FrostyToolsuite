using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.KeyframedPhysicsAnimationEntityGameSpline))]
	public class KeyframedPhysicsAnimationEntityGameSpline : GameSpline, IEntityData<FrostySdk.Ebx.KeyframedPhysicsAnimationEntityGameSpline>
	{
		public new FrostySdk.Ebx.KeyframedPhysicsAnimationEntityGameSpline Data => data as FrostySdk.Ebx.KeyframedPhysicsAnimationEntityGameSpline;
		public override string DisplayName => "KeyframedPhysicsAnimationEntityGameSpline";

		public KeyframedPhysicsAnimationEntityGameSpline(FrostySdk.Ebx.KeyframedPhysicsAnimationEntityGameSpline inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

