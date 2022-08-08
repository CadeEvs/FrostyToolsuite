using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CameraShakeEntityData))]
	public class CameraShakeEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CameraShakeEntityData>
	{
		public new FrostySdk.Ebx.CameraShakeEntityData Data => data as FrostySdk.Ebx.CameraShakeEntityData;
		public override string DisplayName => "CameraShake";

		public CameraShakeEntity(FrostySdk.Ebx.CameraShakeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

