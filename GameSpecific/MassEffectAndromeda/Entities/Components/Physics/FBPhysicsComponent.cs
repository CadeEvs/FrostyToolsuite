using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FBPhysicsComponentData))]
	public class FBPhysicsComponent : Component, IEntityData<FrostySdk.Ebx.FBPhysicsComponentData>
	{
		public new FrostySdk.Ebx.FBPhysicsComponentData Data => data as FrostySdk.Ebx.FBPhysicsComponentData;
		public override string DisplayName => "FBPhysicsComponent";

		public FBPhysicsComponent(FrostySdk.Ebx.FBPhysicsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

