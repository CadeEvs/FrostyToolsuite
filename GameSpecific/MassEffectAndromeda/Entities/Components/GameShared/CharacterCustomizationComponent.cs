using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterCustomizationComponentData))]
	public class CharacterCustomizationComponent : GameComponent, IEntityData<FrostySdk.Ebx.CharacterCustomizationComponentData>
	{
		public new FrostySdk.Ebx.CharacterCustomizationComponentData Data => data as FrostySdk.Ebx.CharacterCustomizationComponentData;
		public override string DisplayName => "CharacterCustomizationComponent";

		public CharacterCustomizationComponent(FrostySdk.Ebx.CharacterCustomizationComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

