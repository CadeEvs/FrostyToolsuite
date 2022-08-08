using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AISearchAreaEntityData))]
	public class AISearchAreaEntity : AIParameterWithShapeEntity, IEntityData<FrostySdk.Ebx.AISearchAreaEntityData>
	{
		public new FrostySdk.Ebx.AISearchAreaEntityData Data => data as FrostySdk.Ebx.AISearchAreaEntityData;
		public override string DisplayName => "AISearchArea";

		public AISearchAreaEntity(FrostySdk.Ebx.AISearchAreaEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

