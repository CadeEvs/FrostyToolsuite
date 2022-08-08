using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CompareVoiceLineIdDataEntityData))]
	public class CompareVoiceLineIdDataEntity : CompareEntityBase, IEntityData<FrostySdk.Ebx.CompareVoiceLineIdDataEntityData>
	{
		public new FrostySdk.Ebx.CompareVoiceLineIdDataEntityData Data => data as FrostySdk.Ebx.CompareVoiceLineIdDataEntityData;
		public override string DisplayName => "CompareVoiceLineIdData";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CompareVoiceLineIdDataEntity(FrostySdk.Ebx.CompareVoiceLineIdDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

