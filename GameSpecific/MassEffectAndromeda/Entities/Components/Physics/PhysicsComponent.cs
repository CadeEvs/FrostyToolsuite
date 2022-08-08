using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PhysicsComponentData))]
	public class PhysicsComponent : Component, IEntityData<FrostySdk.Ebx.PhysicsComponentData>
	{
		public new FrostySdk.Ebx.PhysicsComponentData Data => data as FrostySdk.Ebx.PhysicsComponentData;
		public override string DisplayName => "PhysicsComponent";

		public PhysicsComponent(FrostySdk.Ebx.PhysicsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

