using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIFlankingCorridorEntityData))]
	public class AIFlankingCorridorEntity : AIParameterWithShapeEntity, IEntityData<FrostySdk.Ebx.AIFlankingCorridorEntityData>
	{
		public new FrostySdk.Ebx.AIFlankingCorridorEntityData Data => data as FrostySdk.Ebx.AIFlankingCorridorEntityData;
		public override string DisplayName => "AIFlankingCorridor";

		public AIFlankingCorridorEntity(FrostySdk.Ebx.AIFlankingCorridorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

