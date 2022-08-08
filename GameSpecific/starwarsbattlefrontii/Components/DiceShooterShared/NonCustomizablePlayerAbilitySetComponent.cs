
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.NonCustomizablePlayerAbilitySetComponentData))]
	public class NonCustomizablePlayerAbilitySetComponent : PlayerAbilitySetComponent, IEntityData<FrostySdk.Ebx.NonCustomizablePlayerAbilitySetComponentData>
	{
		public new FrostySdk.Ebx.NonCustomizablePlayerAbilitySetComponentData Data => data as FrostySdk.Ebx.NonCustomizablePlayerAbilitySetComponentData;
		public override string DisplayName => "NonCustomizablePlayerAbilitySetComponent";

		public NonCustomizablePlayerAbilitySetComponent(FrostySdk.Ebx.NonCustomizablePlayerAbilitySetComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

