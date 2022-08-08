using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PrioritizedValueManagerEntityData))]
	public class PrioritizedValueManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PrioritizedValueManagerEntityData>
	{
		public new FrostySdk.Ebx.PrioritizedValueManagerEntityData Data => data as FrostySdk.Ebx.PrioritizedValueManagerEntityData;
		public override string DisplayName => "PrioritizedValueManager";

		public PrioritizedValueManagerEntity(FrostySdk.Ebx.PrioritizedValueManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

