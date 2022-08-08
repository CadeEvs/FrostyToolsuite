
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoldierCameraComponentData))]
	public class SoldierCameraComponent : CharacterCameraComponent, IEntityData<FrostySdk.Ebx.SoldierCameraComponentData>
	{
		public new FrostySdk.Ebx.SoldierCameraComponentData Data => data as FrostySdk.Ebx.SoldierCameraComponentData;
		public override string DisplayName => "SoldierCameraComponent";

		public SoldierCameraComponent(FrostySdk.Ebx.SoldierCameraComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

