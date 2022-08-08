using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OverheatComponentOverrideEntityData))]
	public class OverheatComponentOverrideEntity : LogicEntity, IEntityData<FrostySdk.Ebx.OverheatComponentOverrideEntityData>
	{
		public new FrostySdk.Ebx.OverheatComponentOverrideEntityData Data => data as FrostySdk.Ebx.OverheatComponentOverrideEntityData;
		public override string DisplayName => "OverheatComponentOverride";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public OverheatComponentOverrideEntity(FrostySdk.Ebx.OverheatComponentOverrideEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

