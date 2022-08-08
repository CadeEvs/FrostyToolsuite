using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CombatAreaEntityData))]
	public class CombatAreaEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CombatAreaEntityData>
	{
		public new FrostySdk.Ebx.CombatAreaEntityData Data => data as FrostySdk.Ebx.CombatAreaEntityData;
		public override string DisplayName => "CombatArea";

		public CombatAreaEntity(FrostySdk.Ebx.CombatAreaEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

