using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterSimpleWeaponComponentData))]
	public class CharacterSimpleWeaponComponent : GameComponent, IEntityData<FrostySdk.Ebx.CharacterSimpleWeaponComponentData>
	{
		public new FrostySdk.Ebx.CharacterSimpleWeaponComponentData Data => data as FrostySdk.Ebx.CharacterSimpleWeaponComponentData;
		public override string DisplayName => "CharacterSimpleWeaponComponent";

		public CharacterSimpleWeaponComponent(FrostySdk.Ebx.CharacterSimpleWeaponComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

