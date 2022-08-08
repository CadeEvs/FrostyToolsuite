using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterClassIdCollectionMergerEntityData))]
	public class CharacterClassIdCollectionMergerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CharacterClassIdCollectionMergerEntityData>
	{
		public new FrostySdk.Ebx.CharacterClassIdCollectionMergerEntityData Data => data as FrostySdk.Ebx.CharacterClassIdCollectionMergerEntityData;
		public override string DisplayName => "CharacterClassIdCollectionMerger";

		public CharacterClassIdCollectionMergerEntity(FrostySdk.Ebx.CharacterClassIdCollectionMergerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

