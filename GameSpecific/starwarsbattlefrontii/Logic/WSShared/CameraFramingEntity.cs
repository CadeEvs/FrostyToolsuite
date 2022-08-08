using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CameraFramingEntityData))]
	public class CameraFramingEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CameraFramingEntityData>
	{
		public new FrostySdk.Ebx.CameraFramingEntityData Data => data as FrostySdk.Ebx.CameraFramingEntityData;
		public override string DisplayName => "CameraFraming";

		public CameraFramingEntity(FrostySdk.Ebx.CameraFramingEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

