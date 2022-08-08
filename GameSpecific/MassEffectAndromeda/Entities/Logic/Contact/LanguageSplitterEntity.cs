using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LanguageSplitterEntityData))]
	public class LanguageSplitterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LanguageSplitterEntityData>
	{
		public new FrostySdk.Ebx.LanguageSplitterEntityData Data => data as FrostySdk.Ebx.LanguageSplitterEntityData;
		public override string DisplayName => "LanguageSplitter";

		public LanguageSplitterEntity(FrostySdk.Ebx.LanguageSplitterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

