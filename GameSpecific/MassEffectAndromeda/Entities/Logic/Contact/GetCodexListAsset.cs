using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GetCodexListAssetData))]
	public class GetCodexListAsset : LogicEntity, IEntityData<FrostySdk.Ebx.GetCodexListAssetData>
	{
		public new FrostySdk.Ebx.GetCodexListAssetData Data => data as FrostySdk.Ebx.GetCodexListAssetData;
		public override string DisplayName => "GetCodexListAsset";

		public GetCodexListAsset(FrostySdk.Ebx.GetCodexListAssetData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

