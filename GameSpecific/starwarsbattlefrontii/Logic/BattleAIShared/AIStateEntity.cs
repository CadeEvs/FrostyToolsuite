using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIStateEntityData))]
	public class AIStateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AIStateEntityData>
	{
		public new FrostySdk.Ebx.AIStateEntityData Data => data as FrostySdk.Ebx.AIStateEntityData;
		public override string DisplayName => "AIState";

		public AIStateEntity(FrostySdk.Ebx.AIStateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

