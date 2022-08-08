using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AITacticEntityData))]
	public class AITacticEntity : AIParameterEntity, IEntityData<FrostySdk.Ebx.AITacticEntityData>
	{
		public new FrostySdk.Ebx.AITacticEntityData Data => data as FrostySdk.Ebx.AITacticEntityData;
		public override string DisplayName => "AITactic";

		public AITacticEntity(FrostySdk.Ebx.AITacticEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

