using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UnlockAssetToIndexEntityData))]
	public class UnlockAssetToIndexEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UnlockAssetToIndexEntityData>
	{
		public new FrostySdk.Ebx.UnlockAssetToIndexEntityData Data => data as FrostySdk.Ebx.UnlockAssetToIndexEntityData;
		public override string DisplayName => "UnlockAssetToIndex";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public UnlockAssetToIndexEntity(FrostySdk.Ebx.UnlockAssetToIndexEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

