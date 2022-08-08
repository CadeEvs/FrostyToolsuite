
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HeroNarrativeSystemComponentData))]
	public class HeroNarrativeSystemComponent : CharacterNarrativeSystemComponent, IEntityData<FrostySdk.Ebx.HeroNarrativeSystemComponentData>
	{
		public new FrostySdk.Ebx.HeroNarrativeSystemComponentData Data => data as FrostySdk.Ebx.HeroNarrativeSystemComponentData;
		public override string DisplayName => "HeroNarrativeSystemComponent";

		public HeroNarrativeSystemComponent(FrostySdk.Ebx.HeroNarrativeSystemComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

