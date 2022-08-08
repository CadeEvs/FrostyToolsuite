using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CapturePointEntityData))]
	public class CapturePointEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.CapturePointEntityData>
	{
		public new FrostySdk.Ebx.CapturePointEntityData Data => data as FrostySdk.Ebx.CapturePointEntityData;

		public CapturePointEntity(FrostySdk.Ebx.CapturePointEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

