using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GamePhysicsComponentData))]
	public class GamePhysicsComponent : PhysicsComponent, IEntityData<FrostySdk.Ebx.GamePhysicsComponentData>
	{
		public new FrostySdk.Ebx.GamePhysicsComponentData Data => data as FrostySdk.Ebx.GamePhysicsComponentData;
		public override string DisplayName => "GamePhysicsComponent";

		public GamePhysicsComponent(FrostySdk.Ebx.GamePhysicsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

