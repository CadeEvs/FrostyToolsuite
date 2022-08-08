using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterMasterPhysicsComponentData))]
	public class CharacterMasterPhysicsComponent : ControllablePhysicsComponent, IEntityData<FrostySdk.Ebx.CharacterMasterPhysicsComponentData>
	{
		public new FrostySdk.Ebx.CharacterMasterPhysicsComponentData Data => data as FrostySdk.Ebx.CharacterMasterPhysicsComponentData;
		public override string DisplayName => "CharacterMasterPhysicsComponent";

		public CharacterMasterPhysicsComponent(FrostySdk.Ebx.CharacterMasterPhysicsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

