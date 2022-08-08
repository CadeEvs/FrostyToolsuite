using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinebotCameraEntityData))]
	public class CinebotCameraEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.CinebotCameraEntityData>
	{
		public new FrostySdk.Ebx.CinebotCameraEntityData Data => data as FrostySdk.Ebx.CinebotCameraEntityData;
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("TakeControl", Direction.In),
				new ConnectionDesc("ReleaseControl", Direction.In)
			};
		}

		public CinebotCameraEntity(FrostySdk.Ebx.CinebotCameraEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

