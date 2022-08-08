using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StringSplitterEntityData))]
	public class StringSplitterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.StringSplitterEntityData>
	{
		public new FrostySdk.Ebx.StringSplitterEntityData Data => data as FrostySdk.Ebx.StringSplitterEntityData;
		public override string DisplayName => "StringSplitter";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public StringSplitterEntity(FrostySdk.Ebx.StringSplitterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

