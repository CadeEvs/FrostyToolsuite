using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEProjectileReflectPhysicsComponentData))]
	public class MEProjectileReflectPhysicsComponent : StaticModelPhysicsComponent, IEntityData<FrostySdk.Ebx.MEProjectileReflectPhysicsComponentData>
	{
		public new FrostySdk.Ebx.MEProjectileReflectPhysicsComponentData Data => data as FrostySdk.Ebx.MEProjectileReflectPhysicsComponentData;
		public override string DisplayName => "MEProjectileReflectPhysicsComponent";

		public MEProjectileReflectPhysicsComponent(FrostySdk.Ebx.MEProjectileReflectPhysicsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

