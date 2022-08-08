using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HoverCameraConfigEntityData))]
	public class HoverCameraConfigEntity : LogicEntity, IEntityData<FrostySdk.Ebx.HoverCameraConfigEntityData>
	{
		public new FrostySdk.Ebx.HoverCameraConfigEntityData Data => data as FrostySdk.Ebx.HoverCameraConfigEntityData;
		public override string DisplayName => "HoverCameraConfig";

		public HoverCameraConfigEntity(FrostySdk.Ebx.HoverCameraConfigEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

