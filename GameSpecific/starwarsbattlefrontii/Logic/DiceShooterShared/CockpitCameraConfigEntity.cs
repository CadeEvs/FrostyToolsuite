using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CockpitCameraConfigEntityData))]
	public class CockpitCameraConfigEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CockpitCameraConfigEntityData>
	{
		public new FrostySdk.Ebx.CockpitCameraConfigEntityData Data => data as FrostySdk.Ebx.CockpitCameraConfigEntityData;
		public override string DisplayName => "CockpitCameraConfig";

		public CockpitCameraConfigEntity(FrostySdk.Ebx.CockpitCameraConfigEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

