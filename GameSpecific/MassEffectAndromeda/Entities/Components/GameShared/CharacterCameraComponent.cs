using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterCameraComponentData))]
	public class CharacterCameraComponent : GameComponent, IEntityData<FrostySdk.Ebx.CharacterCameraComponentData>
	{
		public new FrostySdk.Ebx.CharacterCameraComponentData Data => data as FrostySdk.Ebx.CharacterCameraComponentData;
		public override string DisplayName => "CharacterCameraComponent";

		public CharacterCameraComponent(FrostySdk.Ebx.CharacterCameraComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

