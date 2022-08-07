using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TextSequenceEntityData))]
	public class TextSequenceEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TextSequenceEntityData>
	{
		public new FrostySdk.Ebx.TextSequenceEntityData Data => data as FrostySdk.Ebx.TextSequenceEntityData;
		public override string DisplayName => "TextSequence";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public TextSequenceEntity(FrostySdk.Ebx.TextSequenceEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

