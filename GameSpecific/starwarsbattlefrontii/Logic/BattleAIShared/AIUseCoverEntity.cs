using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIUseCoverEntityData))]
	public class AIUseCoverEntity : AIParameterEntity, IEntityData<FrostySdk.Ebx.AIUseCoverEntityData>
	{
		public new FrostySdk.Ebx.AIUseCoverEntityData Data => data as FrostySdk.Ebx.AIUseCoverEntityData;
		public override string DisplayName => "AIUseCover";

		public AIUseCoverEntity(FrostySdk.Ebx.AIUseCoverEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

