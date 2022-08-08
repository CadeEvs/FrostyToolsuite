using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterInputDrivenCameraComponentData))]
	public class CharacterInputDrivenCameraComponent : CharacterCameraComponent, IEntityData<FrostySdk.Ebx.CharacterInputDrivenCameraComponentData>
	{
		public new FrostySdk.Ebx.CharacterInputDrivenCameraComponentData Data => data as FrostySdk.Ebx.CharacterInputDrivenCameraComponentData;
		public override string DisplayName => "CharacterInputDrivenCameraComponent";

		public CharacterInputDrivenCameraComponent(FrostySdk.Ebx.CharacterInputDrivenCameraComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

