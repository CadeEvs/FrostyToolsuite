using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StageEntityData))]
	public class StageEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.StageEntityData>
	{
		public new FrostySdk.Ebx.StageEntityData Data => data as FrostySdk.Ebx.StageEntityData;

		public StageEntity(FrostySdk.Ebx.StageEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

