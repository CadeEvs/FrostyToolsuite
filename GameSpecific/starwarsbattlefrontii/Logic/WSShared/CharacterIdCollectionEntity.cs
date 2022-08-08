using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterIdCollectionEntityData))]
	public class CharacterIdCollectionEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CharacterIdCollectionEntityData>
	{
		public new FrostySdk.Ebx.CharacterIdCollectionEntityData Data => data as FrostySdk.Ebx.CharacterIdCollectionEntityData;
		public override string DisplayName => "CharacterIdCollection";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CharacterIdCollectionEntity(FrostySdk.Ebx.CharacterIdCollectionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

