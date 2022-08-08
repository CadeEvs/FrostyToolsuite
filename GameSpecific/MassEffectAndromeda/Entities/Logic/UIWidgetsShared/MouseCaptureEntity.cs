using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MouseCaptureEntityData))]
	public class MouseCaptureEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MouseCaptureEntityData>
	{
		public new FrostySdk.Ebx.MouseCaptureEntityData Data => data as FrostySdk.Ebx.MouseCaptureEntityData;
		public override string DisplayName => "MouseCapture";
        public override IEnumerable<ConnectionDesc> Links
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Widget", Direction.In)
			};
		}
        public override IEnumerable<ConnectionDesc> Events
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Capture", Direction.In),
				new ConnectionDesc("Release", Direction.In)
			};
		}

        public MouseCaptureEntity(FrostySdk.Ebx.MouseCaptureEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

