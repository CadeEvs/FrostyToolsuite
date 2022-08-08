using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SuggestLeadWaypointPathEntityData))]
	public class SuggestLeadWaypointPathEntity : SuggestFollowWaypointPathEntity, IEntityData<FrostySdk.Ebx.SuggestLeadWaypointPathEntityData>
	{
		public new FrostySdk.Ebx.SuggestLeadWaypointPathEntityData Data => data as FrostySdk.Ebx.SuggestLeadWaypointPathEntityData;
		public override string DisplayName => "SuggestLeadWaypointPath";

		public SuggestLeadWaypointPathEntity(FrostySdk.Ebx.SuggestLeadWaypointPathEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

