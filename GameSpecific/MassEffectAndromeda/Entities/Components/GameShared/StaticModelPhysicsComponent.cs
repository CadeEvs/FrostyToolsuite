using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StaticModelPhysicsComponentData))]
	public class StaticModelPhysicsComponent : GamePhysicsComponent, IEntityData<FrostySdk.Ebx.StaticModelPhysicsComponentData>
	{
		public new FrostySdk.Ebx.StaticModelPhysicsComponentData Data => data as FrostySdk.Ebx.StaticModelPhysicsComponentData;
		public override string DisplayName => "StaticModelPhysicsComponent";

		public StaticModelPhysicsComponent(FrostySdk.Ebx.StaticModelPhysicsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

