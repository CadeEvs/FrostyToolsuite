using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIConcealmentVolumeEntityData))]
	public class AIConcealmentVolumeEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AIConcealmentVolumeEntityData>
	{
		public new FrostySdk.Ebx.AIConcealmentVolumeEntityData Data => data as FrostySdk.Ebx.AIConcealmentVolumeEntityData;
		public override string DisplayName => "AIConcealmentVolume";

		public AIConcealmentVolumeEntity(FrostySdk.Ebx.AIConcealmentVolumeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

