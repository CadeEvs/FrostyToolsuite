using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SphericalCameraTransformationEntityData))]
	public class SphericalCameraTransformationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SphericalCameraTransformationEntityData>
	{
		public new FrostySdk.Ebx.SphericalCameraTransformationEntityData Data => data as FrostySdk.Ebx.SphericalCameraTransformationEntityData;
		public override string DisplayName => "SphericalCameraTransformation";

		public SphericalCameraTransformationEntity(FrostySdk.Ebx.SphericalCameraTransformationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

