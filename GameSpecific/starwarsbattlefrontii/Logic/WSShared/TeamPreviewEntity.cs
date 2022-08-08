using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TeamPreviewEntityData))]
	public class TeamPreviewEntity : TeamEntity, IEntityData<FrostySdk.Ebx.TeamPreviewEntityData>
	{
		public new FrostySdk.Ebx.TeamPreviewEntityData Data => data as FrostySdk.Ebx.TeamPreviewEntityData;
		public override string DisplayName => "TeamPreview";

		public TeamPreviewEntity(FrostySdk.Ebx.TeamPreviewEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

