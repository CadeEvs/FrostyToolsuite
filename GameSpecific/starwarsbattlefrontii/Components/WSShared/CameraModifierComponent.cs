
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CameraModifierComponentData))]
	public class CameraModifierComponent : GameComponent, IEntityData<FrostySdk.Ebx.CameraModifierComponentData>
	{
		public new FrostySdk.Ebx.CameraModifierComponentData Data => data as FrostySdk.Ebx.CameraModifierComponentData;
		public override string DisplayName => "CameraModifierComponent";

		public CameraModifierComponent(FrostySdk.Ebx.CameraModifierComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

