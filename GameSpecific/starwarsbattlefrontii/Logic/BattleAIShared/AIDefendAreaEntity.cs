using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIDefendAreaEntityData))]
	public class AIDefendAreaEntity : AIParameterWithShapeEntity, IEntityData<FrostySdk.Ebx.AIDefendAreaEntityData>
	{
		public new FrostySdk.Ebx.AIDefendAreaEntityData Data => data as FrostySdk.Ebx.AIDefendAreaEntityData;
		public override string DisplayName => "AIDefendArea";

		public AIDefendAreaEntity(FrostySdk.Ebx.AIDefendAreaEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

