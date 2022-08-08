using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ExplosionPackPhysicsComponentData))]
	public class ExplosionPackPhysicsComponent : GamePhysicsComponent, IEntityData<FrostySdk.Ebx.ExplosionPackPhysicsComponentData>
	{
		public new FrostySdk.Ebx.ExplosionPackPhysicsComponentData Data => data as FrostySdk.Ebx.ExplosionPackPhysicsComponentData;
		public override string DisplayName => "ExplosionPackPhysicsComponent";

		public ExplosionPackPhysicsComponent(FrostySdk.Ebx.ExplosionPackPhysicsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

