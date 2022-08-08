using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterIsLocalPlayerEntityData))]
	public class CharacterIsLocalPlayerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CharacterIsLocalPlayerEntityData>
	{
		public new FrostySdk.Ebx.CharacterIsLocalPlayerEntityData Data => data as FrostySdk.Ebx.CharacterIsLocalPlayerEntityData;
		public override string DisplayName => "CharacterIsLocalPlayer";

		public CharacterIsLocalPlayerEntity(FrostySdk.Ebx.CharacterIsLocalPlayerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

