
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CameraComponentData))]
	public class CameraComponent : GameComponent, IEntityData<FrostySdk.Ebx.CameraComponentData>
	{
		public new FrostySdk.Ebx.CameraComponentData Data => data as FrostySdk.Ebx.CameraComponentData;
		public override string DisplayName => "CameraComponent";

		public CameraComponent(FrostySdk.Ebx.CameraComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

