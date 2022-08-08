using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TrackRespawnManagerEntityData))]
	public class TrackRespawnManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TrackRespawnManagerEntityData>
	{
		public new FrostySdk.Ebx.TrackRespawnManagerEntityData Data => data as FrostySdk.Ebx.TrackRespawnManagerEntityData;
		public override string DisplayName => "TrackRespawnManager";

		public TrackRespawnManagerEntity(FrostySdk.Ebx.TrackRespawnManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

