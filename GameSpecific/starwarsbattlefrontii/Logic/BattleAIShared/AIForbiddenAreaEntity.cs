using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIForbiddenAreaEntityData))]
	public class AIForbiddenAreaEntity : AIParameterWithShapeEntity, IEntityData<FrostySdk.Ebx.AIForbiddenAreaEntityData>
	{
		public new FrostySdk.Ebx.AIForbiddenAreaEntityData Data => data as FrostySdk.Ebx.AIForbiddenAreaEntityData;
		public override string DisplayName => "AIForbiddenArea";

		public AIForbiddenAreaEntity(FrostySdk.Ebx.AIForbiddenAreaEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

