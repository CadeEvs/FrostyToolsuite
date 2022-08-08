
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterDefinitionComponentData))]
	public class CharacterDefinitionComponent : GameComponent, IEntityData<FrostySdk.Ebx.CharacterDefinitionComponentData>
	{
		public new FrostySdk.Ebx.CharacterDefinitionComponentData Data => data as FrostySdk.Ebx.CharacterDefinitionComponentData;
		public override string DisplayName => "CharacterDefinitionComponent";

		public CharacterDefinitionComponent(FrostySdk.Ebx.CharacterDefinitionComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

