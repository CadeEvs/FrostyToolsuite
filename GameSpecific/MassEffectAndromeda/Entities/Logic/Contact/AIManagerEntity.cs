using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIManagerEntityData))]
	public class AIManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AIManagerEntityData>
	{
		public new FrostySdk.Ebx.AIManagerEntityData Data => data as FrostySdk.Ebx.AIManagerEntityData;
		public override string DisplayName => "AIManager";

		public AIManagerEntity(FrostySdk.Ebx.AIManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

