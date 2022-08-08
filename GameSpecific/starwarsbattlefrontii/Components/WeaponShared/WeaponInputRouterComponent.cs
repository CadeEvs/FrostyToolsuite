
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WeaponInputRouterComponentData))]
	public class WeaponInputRouterComponent : GameComponent, IEntityData<FrostySdk.Ebx.WeaponInputRouterComponentData>
	{
		public new FrostySdk.Ebx.WeaponInputRouterComponentData Data => data as FrostySdk.Ebx.WeaponInputRouterComponentData;
		public override string DisplayName => "WeaponInputRouterComponent";

		public WeaponInputRouterComponent(FrostySdk.Ebx.WeaponInputRouterComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

