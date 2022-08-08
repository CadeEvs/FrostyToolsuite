using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MECombatControllerData))]
	public class MECombatController : LogicController, IEntityData<FrostySdk.Ebx.MECombatControllerData>
	{
		public new FrostySdk.Ebx.MECombatControllerData Data => data as FrostySdk.Ebx.MECombatControllerData;
		public override string DisplayName => "MECombatController";

		public MECombatController(FrostySdk.Ebx.MECombatControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

