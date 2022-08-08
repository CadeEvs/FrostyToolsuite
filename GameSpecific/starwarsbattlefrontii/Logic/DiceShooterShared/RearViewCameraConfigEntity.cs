using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RearViewCameraConfigEntityData))]
	public class RearViewCameraConfigEntity : LogicEntity, IEntityData<FrostySdk.Ebx.RearViewCameraConfigEntityData>
	{
		public new FrostySdk.Ebx.RearViewCameraConfigEntityData Data => data as FrostySdk.Ebx.RearViewCameraConfigEntityData;
		public override string DisplayName => "RearViewCameraConfig";

		public RearViewCameraConfigEntity(FrostySdk.Ebx.RearViewCameraConfigEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

