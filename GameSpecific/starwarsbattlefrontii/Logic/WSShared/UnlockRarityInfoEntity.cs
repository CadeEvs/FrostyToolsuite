using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UnlockRarityInfoEntityData))]
	public class UnlockRarityInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UnlockRarityInfoEntityData>
	{
		public new FrostySdk.Ebx.UnlockRarityInfoEntityData Data => data as FrostySdk.Ebx.UnlockRarityInfoEntityData;
		public override string DisplayName => "UnlockRarityInfo";

		public UnlockRarityInfoEntity(FrostySdk.Ebx.UnlockRarityInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

