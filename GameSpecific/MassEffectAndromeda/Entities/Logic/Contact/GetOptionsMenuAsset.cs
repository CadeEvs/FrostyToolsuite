using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GetOptionsMenuAssetData))]
	public class GetOptionsMenuAsset : LogicEntity, IEntityData<FrostySdk.Ebx.GetOptionsMenuAssetData>
	{
		public new FrostySdk.Ebx.GetOptionsMenuAssetData Data => data as FrostySdk.Ebx.GetOptionsMenuAssetData;
		public override string DisplayName => "GetOptionsMenuAsset";

		public GetOptionsMenuAsset(FrostySdk.Ebx.GetOptionsMenuAssetData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

