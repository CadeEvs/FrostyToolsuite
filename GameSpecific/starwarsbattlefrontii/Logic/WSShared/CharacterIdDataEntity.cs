using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterIdDataEntityData))]
	public class CharacterIdDataEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CharacterIdDataEntityData>
	{
		public new FrostySdk.Ebx.CharacterIdDataEntityData Data => data as FrostySdk.Ebx.CharacterIdDataEntityData;
		public override string DisplayName => "CharacterIdData";

		public CharacterIdDataEntity(FrostySdk.Ebx.CharacterIdDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

