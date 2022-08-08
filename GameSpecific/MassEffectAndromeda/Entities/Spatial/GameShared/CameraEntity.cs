using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CameraEntityData))]
	public class CameraEntity : CameraEntityBase, IEntityData<FrostySdk.Ebx.CameraEntityData>
	{
		public new FrostySdk.Ebx.CameraEntityData Data => data as FrostySdk.Ebx.CameraEntityData;

		public CameraEntity(FrostySdk.Ebx.CameraEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

