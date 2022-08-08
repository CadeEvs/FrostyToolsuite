using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoldierPhysicsComponentData))]
	public class SoldierPhysicsComponent : CharacterMasterPhysicsComponent, IEntityData<FrostySdk.Ebx.SoldierPhysicsComponentData>
	{
		public new FrostySdk.Ebx.SoldierPhysicsComponentData Data => data as FrostySdk.Ebx.SoldierPhysicsComponentData;
		public override string DisplayName => "SoldierPhysicsComponent";

		public SoldierPhysicsComponent(FrostySdk.Ebx.SoldierPhysicsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

