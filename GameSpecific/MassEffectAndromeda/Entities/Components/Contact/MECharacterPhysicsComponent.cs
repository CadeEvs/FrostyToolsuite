using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MECharacterPhysicsComponentData))]
	public class MECharacterPhysicsComponent : EACharacterPhysicsComponent, IEntityData<FrostySdk.Ebx.MECharacterPhysicsComponentData>
	{
		public new FrostySdk.Ebx.MECharacterPhysicsComponentData Data => data as FrostySdk.Ebx.MECharacterPhysicsComponentData;
		public override string DisplayName => "MECharacterPhysicsComponent";

		public MECharacterPhysicsComponent(FrostySdk.Ebx.MECharacterPhysicsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

