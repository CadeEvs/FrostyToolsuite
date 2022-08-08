using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIParameterEntityData))]
	public class AIParameterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AIParameterEntityData>
	{
		public new FrostySdk.Ebx.AIParameterEntityData Data => data as FrostySdk.Ebx.AIParameterEntityData;
		public override string DisplayName => "AIParameter";

		public AIParameterEntity(FrostySdk.Ebx.AIParameterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

