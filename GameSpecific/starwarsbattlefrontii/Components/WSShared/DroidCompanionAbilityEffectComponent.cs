
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DroidCompanionAbilityEffectComponentData))]
	public class DroidCompanionAbilityEffectComponent : GameComponent, IEntityData<FrostySdk.Ebx.DroidCompanionAbilityEffectComponentData>
	{
		public new FrostySdk.Ebx.DroidCompanionAbilityEffectComponentData Data => data as FrostySdk.Ebx.DroidCompanionAbilityEffectComponentData;
		public override string DisplayName => "DroidCompanionAbilityEffectComponent";

		public DroidCompanionAbilityEffectComponent(FrostySdk.Ebx.DroidCompanionAbilityEffectComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

