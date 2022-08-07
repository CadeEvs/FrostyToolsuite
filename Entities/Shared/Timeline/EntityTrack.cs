
using LevelEditorPlugin.Editors;
using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EntityTrackData))]
	public class EntityTrack : EntityTrackBase, IEntityData<FrostySdk.Ebx.EntityTrackData>
	{
		public new FrostySdk.Ebx.EntityTrackData Data => data as FrostySdk.Ebx.EntityTrackData;
		public override string DisplayName => "EntityTrack";

		public EntityTrack(FrostySdk.Ebx.EntityTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
			if (Data.GuidChain.Count > 0)
			{
				trackName = Data.GuidChain[0].ToString();
			}
		}
	}
}

