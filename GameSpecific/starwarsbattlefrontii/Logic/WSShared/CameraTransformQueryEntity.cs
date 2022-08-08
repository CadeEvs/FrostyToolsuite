using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CameraTransformQueryEntityData))]
	public class CameraTransformQueryEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CameraTransformQueryEntityData>
	{
		public new FrostySdk.Ebx.CameraTransformQueryEntityData Data => data as FrostySdk.Ebx.CameraTransformQueryEntityData;
		public override string DisplayName => "CameraTransformQuery";

		public CameraTransformQueryEntity(FrostySdk.Ebx.CameraTransformQueryEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

