using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIGetSuspiciousFactorEntityData))]
	public class AIGetSuspiciousFactorEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AIGetSuspiciousFactorEntityData>
	{
		public new FrostySdk.Ebx.AIGetSuspiciousFactorEntityData Data => data as FrostySdk.Ebx.AIGetSuspiciousFactorEntityData;
		public override string DisplayName => "AIGetSuspiciousFactor";

		public AIGetSuspiciousFactorEntity(FrostySdk.Ebx.AIGetSuspiciousFactorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

