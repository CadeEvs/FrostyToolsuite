
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SPAIPlayerAbilitySetComponentData))]
	public class SPAIPlayerAbilitySetComponent : WSNonCustomizablePlayerAbilitySetComponent, IEntityData<FrostySdk.Ebx.SPAIPlayerAbilitySetComponentData>
	{
		public new FrostySdk.Ebx.SPAIPlayerAbilitySetComponentData Data => data as FrostySdk.Ebx.SPAIPlayerAbilitySetComponentData;
		public override string DisplayName => "SPAIPlayerAbilitySetComponent";

		public SPAIPlayerAbilitySetComponent(FrostySdk.Ebx.SPAIPlayerAbilitySetComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

