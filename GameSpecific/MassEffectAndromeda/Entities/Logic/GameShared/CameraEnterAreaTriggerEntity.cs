using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CameraEnterAreaTriggerEntityData))]
	public class CameraEnterAreaTriggerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CameraEnterAreaTriggerEntityData>
	{
		public new FrostySdk.Ebx.CameraEnterAreaTriggerEntityData Data => data as FrostySdk.Ebx.CameraEnterAreaTriggerEntityData;
		public override string DisplayName => "CameraEnterAreaTrigger";

		public CameraEnterAreaTriggerEntity(FrostySdk.Ebx.CameraEnterAreaTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

