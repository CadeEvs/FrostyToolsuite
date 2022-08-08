using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PersistenceDailyLootBoxStatusEntityData))]
	public class PersistenceDailyLootBoxStatusEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PersistenceDailyLootBoxStatusEntityData>
	{
		public new FrostySdk.Ebx.PersistenceDailyLootBoxStatusEntityData Data => data as FrostySdk.Ebx.PersistenceDailyLootBoxStatusEntityData;
		public override string DisplayName => "PersistenceDailyLootBoxStatus";

		public PersistenceDailyLootBoxStatusEntity(FrostySdk.Ebx.PersistenceDailyLootBoxStatusEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

