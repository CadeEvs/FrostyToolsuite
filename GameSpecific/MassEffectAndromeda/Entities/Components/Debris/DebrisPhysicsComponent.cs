using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DebrisPhysicsComponentData))]
	public class DebrisPhysicsComponent : PhysicsComponent, IEntityData<FrostySdk.Ebx.DebrisPhysicsComponentData>
	{
		public new FrostySdk.Ebx.DebrisPhysicsComponentData Data => data as FrostySdk.Ebx.DebrisPhysicsComponentData;
		public override string DisplayName => "DebrisPhysicsComponent";

		public DebrisPhysicsComponent(FrostySdk.Ebx.DebrisPhysicsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

