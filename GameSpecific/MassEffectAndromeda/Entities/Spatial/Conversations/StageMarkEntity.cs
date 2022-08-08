using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StageMarkEntityData))]
	public class StageMarkEntity : StageObjectEntity, IEntityData<FrostySdk.Ebx.StageMarkEntityData>
	{
		public new FrostySdk.Ebx.StageMarkEntityData Data => data as FrostySdk.Ebx.StageMarkEntityData;

		public StageMarkEntity(FrostySdk.Ebx.StageMarkEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

