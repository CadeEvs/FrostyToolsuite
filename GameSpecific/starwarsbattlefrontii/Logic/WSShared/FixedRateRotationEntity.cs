using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FixedRateRotationEntityData))]
	public class FixedRateRotationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.FixedRateRotationEntityData>
	{
		public new FrostySdk.Ebx.FixedRateRotationEntityData Data => data as FrostySdk.Ebx.FixedRateRotationEntityData;
		public override string DisplayName => "FixedRateRotation";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public FixedRateRotationEntity(FrostySdk.Ebx.FixedRateRotationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

