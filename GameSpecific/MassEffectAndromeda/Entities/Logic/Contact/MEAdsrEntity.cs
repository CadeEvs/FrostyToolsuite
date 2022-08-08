using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEAdsrEntityData))]
	public class MEAdsrEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MEAdsrEntityData>
	{
		public new FrostySdk.Ebx.MEAdsrEntityData Data => data as FrostySdk.Ebx.MEAdsrEntityData;
		public override string DisplayName => "MEAdsr";

		public MEAdsrEntity(FrostySdk.Ebx.MEAdsrEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

