using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TurretFirePatternOverrideEntityData))]
	public class TurretFirePatternOverrideEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TurretFirePatternOverrideEntityData>
	{
		public new FrostySdk.Ebx.TurretFirePatternOverrideEntityData Data => data as FrostySdk.Ebx.TurretFirePatternOverrideEntityData;
		public override string DisplayName => "TurretFirePatternOverride";

		public TurretFirePatternOverrideEntity(FrostySdk.Ebx.TurretFirePatternOverrideEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

