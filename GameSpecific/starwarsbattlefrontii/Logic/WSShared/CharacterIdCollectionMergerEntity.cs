using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterIdCollectionMergerEntityData))]
	public class CharacterIdCollectionMergerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CharacterIdCollectionMergerEntityData>
	{
		public new FrostySdk.Ebx.CharacterIdCollectionMergerEntityData Data => data as FrostySdk.Ebx.CharacterIdCollectionMergerEntityData;
		public override string DisplayName => "CharacterIdCollectionMerger";

		public CharacterIdCollectionMergerEntity(FrostySdk.Ebx.CharacterIdCollectionMergerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

