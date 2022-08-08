using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SuggestFollowWaypointPathEntityData))]
	public class SuggestFollowWaypointPathEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SuggestFollowWaypointPathEntityData>
	{
		public new FrostySdk.Ebx.SuggestFollowWaypointPathEntityData Data => data as FrostySdk.Ebx.SuggestFollowWaypointPathEntityData;
		public override string DisplayName => "SuggestFollowWaypointPath";

		public SuggestFollowWaypointPathEntity(FrostySdk.Ebx.SuggestFollowWaypointPathEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

