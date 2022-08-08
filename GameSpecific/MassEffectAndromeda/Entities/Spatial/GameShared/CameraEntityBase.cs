using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CameraEntityBaseData))]
	public class CameraEntityBase : SpatialEntity, IEntityData<FrostySdk.Ebx.CameraEntityBaseData>
	{
		public new FrostySdk.Ebx.CameraEntityBaseData Data => data as FrostySdk.Ebx.CameraEntityBaseData;

		public CameraEntityBase(FrostySdk.Ebx.CameraEntityBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

