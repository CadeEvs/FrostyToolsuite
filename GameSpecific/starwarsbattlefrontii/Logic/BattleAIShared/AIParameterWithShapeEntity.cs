using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIParameterWithShapeEntityData))]
	public class AIParameterWithShapeEntity : AIParameterEntity, IEntityData<FrostySdk.Ebx.AIParameterWithShapeEntityData>
	{
		public new FrostySdk.Ebx.AIParameterWithShapeEntityData Data => data as FrostySdk.Ebx.AIParameterWithShapeEntityData;
		public override string DisplayName => "AIParameterWithShape";

		public AIParameterWithShapeEntity(FrostySdk.Ebx.AIParameterWithShapeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

