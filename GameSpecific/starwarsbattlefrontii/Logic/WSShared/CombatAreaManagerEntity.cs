using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CombatAreaManagerEntityData))]
	public class CombatAreaManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CombatAreaManagerEntityData>
	{
		public new FrostySdk.Ebx.CombatAreaManagerEntityData Data => data as FrostySdk.Ebx.CombatAreaManagerEntityData;
		public override string DisplayName => "CombatAreaManager";

		public CombatAreaManagerEntity(FrostySdk.Ebx.CombatAreaManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

