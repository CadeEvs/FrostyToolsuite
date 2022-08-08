using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LookAtCameraEntityData))]
	public class LookAtCameraEntity : CameraEntity, IEntityData<FrostySdk.Ebx.LookAtCameraEntityData>
	{
		public new FrostySdk.Ebx.LookAtCameraEntityData Data => data as FrostySdk.Ebx.LookAtCameraEntityData;
		public override IEnumerable<ConnectionDesc> Links
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Target", Direction.In)
			};
		}
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("TakeControl", Direction.In),
				new ConnectionDesc("ReleaseControl", Direction.In)
			};
		}

		public LookAtCameraEntity(FrostySdk.Ebx.LookAtCameraEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

