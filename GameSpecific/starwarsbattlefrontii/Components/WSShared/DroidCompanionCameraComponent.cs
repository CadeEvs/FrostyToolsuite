
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DroidCompanionCameraComponentData))]
	public class DroidCompanionCameraComponent : GameComponent, IEntityData<FrostySdk.Ebx.DroidCompanionCameraComponentData>
	{
		public new FrostySdk.Ebx.DroidCompanionCameraComponentData Data => data as FrostySdk.Ebx.DroidCompanionCameraComponentData;
		public override string DisplayName => "DroidCompanionCameraComponent";

		public DroidCompanionCameraComponent(FrostySdk.Ebx.DroidCompanionCameraComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

