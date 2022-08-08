using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIFollowAreaEntityData))]
	public class AIFollowAreaEntity : AIParameterWithShapeEntity, IEntityData<FrostySdk.Ebx.AIFollowAreaEntityData>
	{
		public new FrostySdk.Ebx.AIFollowAreaEntityData Data => data as FrostySdk.Ebx.AIFollowAreaEntityData;
		public override string DisplayName => "AIFollowArea";

		public AIFollowAreaEntity(FrostySdk.Ebx.AIFollowAreaEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

