using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UnlockAssetGuidEntityData))]
	public class UnlockAssetGuidEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UnlockAssetGuidEntityData>
	{
		public new FrostySdk.Ebx.UnlockAssetGuidEntityData Data => data as FrostySdk.Ebx.UnlockAssetGuidEntityData;
		public override string DisplayName => "UnlockAssetGuid";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public UnlockAssetGuidEntity(FrostySdk.Ebx.UnlockAssetGuidEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

