using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AICombatGroupEntityData))]
	public class AICombatGroupEntity : AIParameterEntity, IEntityData<FrostySdk.Ebx.AICombatGroupEntityData>
	{
		public new FrostySdk.Ebx.AICombatGroupEntityData Data => data as FrostySdk.Ebx.AICombatGroupEntityData;
		public override string DisplayName => "AICombatGroup";

		public AICombatGroupEntity(FrostySdk.Ebx.AICombatGroupEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

