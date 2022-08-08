using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ContentUpdateDataProviderData))]
	public class ContentUpdateDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.ContentUpdateDataProviderData>
	{
		public new FrostySdk.Ebx.ContentUpdateDataProviderData Data => data as FrostySdk.Ebx.ContentUpdateDataProviderData;
		public override string DisplayName => "ContentUpdateDataProvider";

		public ContentUpdateDataProvider(FrostySdk.Ebx.ContentUpdateDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

