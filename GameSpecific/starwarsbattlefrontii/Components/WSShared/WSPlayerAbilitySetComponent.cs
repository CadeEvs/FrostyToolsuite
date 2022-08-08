
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSPlayerAbilitySetComponentData))]
	public class WSPlayerAbilitySetComponent : PlayerAbilitySetComponent, IEntityData<FrostySdk.Ebx.WSPlayerAbilitySetComponentData>
	{
		public new FrostySdk.Ebx.WSPlayerAbilitySetComponentData Data => data as FrostySdk.Ebx.WSPlayerAbilitySetComponentData;
		public override string DisplayName => "WSPlayerAbilitySetComponent";

		public WSPlayerAbilitySetComponent(FrostySdk.Ebx.WSPlayerAbilitySetComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

