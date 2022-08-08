using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BonusScoreEntityData))]
	public class BonusScoreEntity : LogicEntity, IEntityData<FrostySdk.Ebx.BonusScoreEntityData>
	{
		public new FrostySdk.Ebx.BonusScoreEntityData Data => data as FrostySdk.Ebx.BonusScoreEntityData;
		public override string DisplayName => "BonusScore";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public BonusScoreEntity(FrostySdk.Ebx.BonusScoreEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

