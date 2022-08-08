using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIKillCounterEntityData))]
	public class AIKillCounterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AIKillCounterEntityData>
	{
		public new FrostySdk.Ebx.AIKillCounterEntityData Data => data as FrostySdk.Ebx.AIKillCounterEntityData;
		public override string DisplayName => "AIKillCounter";

		public AIKillCounterEntity(FrostySdk.Ebx.AIKillCounterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

