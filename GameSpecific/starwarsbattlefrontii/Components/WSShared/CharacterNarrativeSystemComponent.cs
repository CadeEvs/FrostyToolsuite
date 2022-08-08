
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterNarrativeSystemComponentData))]
	public class CharacterNarrativeSystemComponent : GameComponent, IEntityData<FrostySdk.Ebx.CharacterNarrativeSystemComponentData>
	{
		public new FrostySdk.Ebx.CharacterNarrativeSystemComponentData Data => data as FrostySdk.Ebx.CharacterNarrativeSystemComponentData;
		public override string DisplayName => "CharacterNarrativeSystemComponent";

		public CharacterNarrativeSystemComponent(FrostySdk.Ebx.CharacterNarrativeSystemComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

