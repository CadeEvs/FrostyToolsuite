
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterLightingComponentData))]
	public class CharacterLightingComponent : VisualEnvironmentComponent, IEntityData<FrostySdk.Ebx.CharacterLightingComponentData>
	{
		public new FrostySdk.Ebx.CharacterLightingComponentData Data => data as FrostySdk.Ebx.CharacterLightingComponentData;
		public override string DisplayName => "CharacterLightingComponent";

		public CharacterLightingComponent(FrostySdk.Ebx.CharacterLightingComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

