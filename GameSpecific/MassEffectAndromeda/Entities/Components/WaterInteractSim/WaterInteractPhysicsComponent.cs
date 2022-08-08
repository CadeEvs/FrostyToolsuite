using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WaterInteractPhysicsComponentData))]
	public class WaterInteractPhysicsComponent : GamePhysicsComponent, IEntityData<FrostySdk.Ebx.WaterInteractPhysicsComponentData>
	{
		public new FrostySdk.Ebx.WaterInteractPhysicsComponentData Data => data as FrostySdk.Ebx.WaterInteractPhysicsComponentData;
		public override string DisplayName => "WaterInteractPhysicsComponent";

		public WaterInteractPhysicsComponent(FrostySdk.Ebx.WaterInteractPhysicsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

