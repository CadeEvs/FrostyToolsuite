using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIAwarenessEntityData))]
	public class AIAwarenessEntity : AIParameterEntity, IEntityData<FrostySdk.Ebx.AIAwarenessEntityData>
	{
		public new FrostySdk.Ebx.AIAwarenessEntityData Data => data as FrostySdk.Ebx.AIAwarenessEntityData;
		public override string DisplayName => "AIAwareness";

		public AIAwarenessEntity(FrostySdk.Ebx.AIAwarenessEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

