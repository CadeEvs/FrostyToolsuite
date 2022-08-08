using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterKitIdCollectionEntityData))]
	public class CharacterKitIdCollectionEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CharacterKitIdCollectionEntityData>
	{
		public new FrostySdk.Ebx.CharacterKitIdCollectionEntityData Data => data as FrostySdk.Ebx.CharacterKitIdCollectionEntityData;
		public override string DisplayName => "CharacterKitIdCollection";

		public CharacterKitIdCollectionEntity(FrostySdk.Ebx.CharacterKitIdCollectionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

