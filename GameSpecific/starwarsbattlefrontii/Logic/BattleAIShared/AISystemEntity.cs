using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AISystemEntityData))]
	public class AISystemEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AISystemEntityData>
	{
		public new FrostySdk.Ebx.AISystemEntityData Data => data as FrostySdk.Ebx.AISystemEntityData;
		public override string DisplayName => "AISystem";

		public AISystemEntity(FrostySdk.Ebx.AISystemEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

