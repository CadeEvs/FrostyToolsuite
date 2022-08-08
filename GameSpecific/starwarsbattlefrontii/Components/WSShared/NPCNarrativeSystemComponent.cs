
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.NPCNarrativeSystemComponentData))]
	public class NPCNarrativeSystemComponent : CharacterNarrativeSystemComponent, IEntityData<FrostySdk.Ebx.NPCNarrativeSystemComponentData>
	{
		public new FrostySdk.Ebx.NPCNarrativeSystemComponentData Data => data as FrostySdk.Ebx.NPCNarrativeSystemComponentData;
		public override string DisplayName => "NPCNarrativeSystemComponent";

		public NPCNarrativeSystemComponent(FrostySdk.Ebx.NPCNarrativeSystemComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

