using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEPlayerCombatStateQueryEntityData))]
	public class MEPlayerCombatStateQueryEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MEPlayerCombatStateQueryEntityData>
	{
		public new FrostySdk.Ebx.MEPlayerCombatStateQueryEntityData Data => data as FrostySdk.Ebx.MEPlayerCombatStateQueryEntityData;
		public override string DisplayName => "MEPlayerCombatStateQuery";

		public MEPlayerCombatStateQueryEntity(FrostySdk.Ebx.MEPlayerCombatStateQueryEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

