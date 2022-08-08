
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AimingComponentData))]
	public class AimingComponent : GameComponent, IEntityData<FrostySdk.Ebx.AimingComponentData>
	{
		public new FrostySdk.Ebx.AimingComponentData Data => data as FrostySdk.Ebx.AimingComponentData;
		public override string DisplayName => "AimingComponent";

		public AimingComponent(FrostySdk.Ebx.AimingComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

