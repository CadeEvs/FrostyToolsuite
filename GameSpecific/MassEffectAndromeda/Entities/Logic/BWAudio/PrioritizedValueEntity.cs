using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PrioritizedValueEntityData))]
	public class PrioritizedValueEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PrioritizedValueEntityData>
	{
		public new FrostySdk.Ebx.PrioritizedValueEntityData Data => data as FrostySdk.Ebx.PrioritizedValueEntityData;
		public override string DisplayName => "PrioritizedValue";

		public PrioritizedValueEntity(FrostySdk.Ebx.PrioritizedValueEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

