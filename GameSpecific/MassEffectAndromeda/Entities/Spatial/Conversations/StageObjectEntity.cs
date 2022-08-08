using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StageObjectEntityData))]
	public class StageObjectEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.StageObjectEntityData>
	{
		public new FrostySdk.Ebx.StageObjectEntityData Data => data as FrostySdk.Ebx.StageObjectEntityData;

		public StageObjectEntity(FrostySdk.Ebx.StageObjectEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

