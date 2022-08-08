using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterStateChannelReaderEntityData))]
	public class CharacterStateChannelReaderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CharacterStateChannelReaderEntityData>
	{
		public new FrostySdk.Ebx.CharacterStateChannelReaderEntityData Data => data as FrostySdk.Ebx.CharacterStateChannelReaderEntityData;
		public override string DisplayName => "CharacterStateChannelReader";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CharacterStateChannelReaderEntity(FrostySdk.Ebx.CharacterStateChannelReaderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

