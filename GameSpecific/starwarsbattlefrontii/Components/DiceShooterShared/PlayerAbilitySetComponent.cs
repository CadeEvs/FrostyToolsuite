
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerAbilitySetComponentData))]
	public class PlayerAbilitySetComponent : GameComponent, IEntityData<FrostySdk.Ebx.PlayerAbilitySetComponentData>
	{
		public new FrostySdk.Ebx.PlayerAbilitySetComponentData Data => data as FrostySdk.Ebx.PlayerAbilitySetComponentData;
		public override string DisplayName => "PlayerAbilitySetComponent";

		public PlayerAbilitySetComponent(FrostySdk.Ebx.PlayerAbilitySetComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

