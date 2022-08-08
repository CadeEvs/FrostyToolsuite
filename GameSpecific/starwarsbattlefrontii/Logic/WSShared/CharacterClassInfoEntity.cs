using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterClassInfoEntityData))]
	public class CharacterClassInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CharacterClassInfoEntityData>
	{
		public new FrostySdk.Ebx.CharacterClassInfoEntityData Data => data as FrostySdk.Ebx.CharacterClassInfoEntityData;
		public override string DisplayName => "CharacterClassInfo";

		public CharacterClassInfoEntity(FrostySdk.Ebx.CharacterClassInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

