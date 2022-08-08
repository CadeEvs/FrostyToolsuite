using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AnimationTurretRotationComponentData))]
	public class AnimationTurretRotationComponent : GameComponent, IEntityData<FrostySdk.Ebx.AnimationTurretRotationComponentData>
	{
		public new FrostySdk.Ebx.AnimationTurretRotationComponentData Data => data as FrostySdk.Ebx.AnimationTurretRotationComponentData;
		public override string DisplayName => "AnimationTurretRotationComponent";

		public AnimationTurretRotationComponent(FrostySdk.Ebx.AnimationTurretRotationComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

