using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterInfoEntityData))]
	public class CharacterInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CharacterInfoEntityData>
	{
		public new FrostySdk.Ebx.CharacterInfoEntityData Data => data as FrostySdk.Ebx.CharacterInfoEntityData;
		public override string DisplayName => "CharacterInfo";

		public CharacterInfoEntity(FrostySdk.Ebx.CharacterInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

