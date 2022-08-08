using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HeadDataCollectionProviderData))]
	public class HeadDataCollectionProvider : LogicEntity, IEntityData<FrostySdk.Ebx.HeadDataCollectionProviderData>
	{
		public new FrostySdk.Ebx.HeadDataCollectionProviderData Data => data as FrostySdk.Ebx.HeadDataCollectionProviderData;
		public override string DisplayName => "HeadDataCollectionProvider";

		public HeadDataCollectionProvider(FrostySdk.Ebx.HeadDataCollectionProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

