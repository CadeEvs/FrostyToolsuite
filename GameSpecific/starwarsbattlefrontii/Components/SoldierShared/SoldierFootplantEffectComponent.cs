
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoldierFootplantEffectComponentData))]
	public class SoldierFootplantEffectComponent : GameComponent, IEntityData<FrostySdk.Ebx.SoldierFootplantEffectComponentData>
	{
		public new FrostySdk.Ebx.SoldierFootplantEffectComponentData Data => data as FrostySdk.Ebx.SoldierFootplantEffectComponentData;
		public override string DisplayName => "SoldierFootplantEffectComponent";

		public SoldierFootplantEffectComponent(FrostySdk.Ebx.SoldierFootplantEffectComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

