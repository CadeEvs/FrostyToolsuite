using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.URLExtractorEntityData))]
	public class URLExtractorEntity : LogicEntity, IEntityData<FrostySdk.Ebx.URLExtractorEntityData>
	{
		public new FrostySdk.Ebx.URLExtractorEntityData Data => data as FrostySdk.Ebx.URLExtractorEntityData;
		public override string DisplayName => "URLExtractor";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public URLExtractorEntity(FrostySdk.Ebx.URLExtractorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

