
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSNonCustomizablePlayerAbilitySetComponentData))]
	public class WSNonCustomizablePlayerAbilitySetComponent : NonCustomizablePlayerAbilitySetComponent, IEntityData<FrostySdk.Ebx.WSNonCustomizablePlayerAbilitySetComponentData>
	{
		public new FrostySdk.Ebx.WSNonCustomizablePlayerAbilitySetComponentData Data => data as FrostySdk.Ebx.WSNonCustomizablePlayerAbilitySetComponentData;
		public override string DisplayName => "WSNonCustomizablePlayerAbilitySetComponent";

		public WSNonCustomizablePlayerAbilitySetComponent(FrostySdk.Ebx.WSNonCustomizablePlayerAbilitySetComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

