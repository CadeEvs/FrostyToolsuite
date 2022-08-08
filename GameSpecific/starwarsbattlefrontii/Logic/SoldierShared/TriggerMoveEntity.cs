using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TriggerMoveEntityData))]
	public class TriggerMoveEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TriggerMoveEntityData>
	{
		public new FrostySdk.Ebx.TriggerMoveEntityData Data => data as FrostySdk.Ebx.TriggerMoveEntityData;
		public override string DisplayName => "TriggerMove";

		public TriggerMoveEntity(FrostySdk.Ebx.TriggerMoveEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

