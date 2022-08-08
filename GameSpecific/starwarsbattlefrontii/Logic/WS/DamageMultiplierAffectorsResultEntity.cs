using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DamageMultiplierAffectorsResultEntityData))]
	public class DamageMultiplierAffectorsResultEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DamageMultiplierAffectorsResultEntityData>
	{
		public new FrostySdk.Ebx.DamageMultiplierAffectorsResultEntityData Data => data as FrostySdk.Ebx.DamageMultiplierAffectorsResultEntityData;
		public override string DisplayName => "DamageMultiplierAffectorsResult";

		public DamageMultiplierAffectorsResultEntity(FrostySdk.Ebx.DamageMultiplierAffectorsResultEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

