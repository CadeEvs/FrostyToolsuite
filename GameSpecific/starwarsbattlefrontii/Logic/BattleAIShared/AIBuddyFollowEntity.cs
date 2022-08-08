using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIBuddyFollowEntityData))]
	public class AIBuddyFollowEntity : AIParameterEntity, IEntityData<FrostySdk.Ebx.AIBuddyFollowEntityData>
	{
		public new FrostySdk.Ebx.AIBuddyFollowEntityData Data => data as FrostySdk.Ebx.AIBuddyFollowEntityData;
		public override string DisplayName => "AIBuddyFollow";

		public AIBuddyFollowEntity(FrostySdk.Ebx.AIBuddyFollowEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

