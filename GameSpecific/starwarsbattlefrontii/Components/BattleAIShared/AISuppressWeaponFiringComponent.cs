
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AISuppressWeaponFiringComponentData))]
	public class AISuppressWeaponFiringComponent : GameComponent, IEntityData<FrostySdk.Ebx.AISuppressWeaponFiringComponentData>
	{
		public new FrostySdk.Ebx.AISuppressWeaponFiringComponentData Data => data as FrostySdk.Ebx.AISuppressWeaponFiringComponentData;
		public override string DisplayName => "AISuppressWeaponFiringComponent";

		public AISuppressWeaponFiringComponent(FrostySdk.Ebx.AISuppressWeaponFiringComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

