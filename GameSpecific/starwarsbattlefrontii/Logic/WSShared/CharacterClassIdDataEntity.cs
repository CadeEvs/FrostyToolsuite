using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterClassIdDataEntityData))]
	public class CharacterClassIdDataEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CharacterClassIdDataEntityData>
	{
		public new FrostySdk.Ebx.CharacterClassIdDataEntityData Data => data as FrostySdk.Ebx.CharacterClassIdDataEntityData;
		public override string DisplayName => "CharacterClassIdData";

		public CharacterClassIdDataEntity(FrostySdk.Ebx.CharacterClassIdDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

