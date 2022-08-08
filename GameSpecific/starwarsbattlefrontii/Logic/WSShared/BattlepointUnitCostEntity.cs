using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BattlepointUnitCostEntityData))]
	public class BattlepointUnitCostEntity : LogicEntity, IEntityData<FrostySdk.Ebx.BattlepointUnitCostEntityData>
	{
		public new FrostySdk.Ebx.BattlepointUnitCostEntityData Data => data as FrostySdk.Ebx.BattlepointUnitCostEntityData;
		public override string DisplayName => "BattlepointUnitCost";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public BattlepointUnitCostEntity(FrostySdk.Ebx.BattlepointUnitCostEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

