using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIPlayerProviderEntityData))]
	public class AIPlayerProviderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AIPlayerProviderEntityData>
	{
		public new FrostySdk.Ebx.AIPlayerProviderEntityData Data => data as FrostySdk.Ebx.AIPlayerProviderEntityData;
		public override string DisplayName => "AIPlayerProvider";

		public AIPlayerProviderEntity(FrostySdk.Ebx.AIPlayerProviderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

