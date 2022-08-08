using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BangerPhysicsComponentData))]
	public class BangerPhysicsComponent : GamePhysicsComponent, IEntityData<FrostySdk.Ebx.BangerPhysicsComponentData>
	{
		public new FrostySdk.Ebx.BangerPhysicsComponentData Data => data as FrostySdk.Ebx.BangerPhysicsComponentData;
		public override string DisplayName => "BangerPhysicsComponent";

		public BangerPhysicsComponent(FrostySdk.Ebx.BangerPhysicsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

