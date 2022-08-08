
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterHealthComponentData))]
	public class CharacterHealthComponent : ControllableHealthComponent, IEntityData<FrostySdk.Ebx.CharacterHealthComponentData>
	{
		public new FrostySdk.Ebx.CharacterHealthComponentData Data => data as FrostySdk.Ebx.CharacterHealthComponentData;
		public override string DisplayName => "CharacterHealthComponent";

		public CharacterHealthComponent(FrostySdk.Ebx.CharacterHealthComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

