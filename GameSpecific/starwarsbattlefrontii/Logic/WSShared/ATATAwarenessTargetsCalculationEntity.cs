using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ATATAwarenessTargetsCalculationEntityData))]
	public class ATATAwarenessTargetsCalculationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ATATAwarenessTargetsCalculationEntityData>
	{
		public new FrostySdk.Ebx.ATATAwarenessTargetsCalculationEntityData Data => data as FrostySdk.Ebx.ATATAwarenessTargetsCalculationEntityData;
		public override string DisplayName => "ATATAwarenessTargetsCalculation";

		public ATATAwarenessTargetsCalculationEntity(FrostySdk.Ebx.ATATAwarenessTargetsCalculationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

