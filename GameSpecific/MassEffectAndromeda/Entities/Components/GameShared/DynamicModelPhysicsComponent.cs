using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DynamicModelPhysicsComponentData))]
	public class DynamicModelPhysicsComponent : GamePhysicsComponent, IEntityData<FrostySdk.Ebx.DynamicModelPhysicsComponentData>
	{
		public new FrostySdk.Ebx.DynamicModelPhysicsComponentData Data => data as FrostySdk.Ebx.DynamicModelPhysicsComponentData;
		public override string DisplayName => "DynamicModelPhysicsComponent";

		public DynamicModelPhysicsComponent(FrostySdk.Ebx.DynamicModelPhysicsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

