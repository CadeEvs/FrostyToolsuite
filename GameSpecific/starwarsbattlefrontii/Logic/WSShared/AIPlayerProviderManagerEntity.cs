using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIPlayerProviderManagerEntityData))]
	public class AIPlayerProviderManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AIPlayerProviderManagerEntityData>
	{
		public new FrostySdk.Ebx.AIPlayerProviderManagerEntityData Data => data as FrostySdk.Ebx.AIPlayerProviderManagerEntityData;
		public override string DisplayName => "AIPlayerProviderManager";

		public AIPlayerProviderManagerEntity(FrostySdk.Ebx.AIPlayerProviderManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

