using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FollowObjectEntityData))]
	public class FollowObjectEntity : LogicEntity, IEntityData<FrostySdk.Ebx.FollowObjectEntityData>
	{
		public new FrostySdk.Ebx.FollowObjectEntityData Data => data as FrostySdk.Ebx.FollowObjectEntityData;
		public override string DisplayName => "FollowObject";

		public FollowObjectEntity(FrostySdk.Ebx.FollowObjectEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

