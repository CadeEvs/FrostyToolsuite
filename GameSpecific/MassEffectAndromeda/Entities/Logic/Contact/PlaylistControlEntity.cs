using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlaylistControlEntityData))]
	public class PlaylistControlEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlaylistControlEntityData>
	{
		public new FrostySdk.Ebx.PlaylistControlEntityData Data => data as FrostySdk.Ebx.PlaylistControlEntityData;
		public override string DisplayName => "PlaylistControl";

		public PlaylistControlEntity(FrostySdk.Ebx.PlaylistControlEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

