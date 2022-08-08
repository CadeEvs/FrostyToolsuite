using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIFollowObjectEntityData))]
	public class AIFollowObjectEntity : AIParameterEntity, IEntityData<FrostySdk.Ebx.AIFollowObjectEntityData>
	{
		public new FrostySdk.Ebx.AIFollowObjectEntityData Data => data as FrostySdk.Ebx.AIFollowObjectEntityData;
		public override string DisplayName => "AIFollowObject";

		public AIFollowObjectEntity(FrostySdk.Ebx.AIFollowObjectEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

