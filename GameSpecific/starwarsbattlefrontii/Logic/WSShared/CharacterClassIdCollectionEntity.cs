using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterClassIdCollectionEntityData))]
	public class CharacterClassIdCollectionEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CharacterClassIdCollectionEntityData>
	{
		public new FrostySdk.Ebx.CharacterClassIdCollectionEntityData Data => data as FrostySdk.Ebx.CharacterClassIdCollectionEntityData;
		public override string DisplayName => "CharacterClassIdCollection";

		public CharacterClassIdCollectionEntity(FrostySdk.Ebx.CharacterClassIdCollectionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

