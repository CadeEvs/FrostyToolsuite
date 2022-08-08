using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ScatterContentSpawnBlockerEntityData))]
	public class ScatterContentSpawnBlockerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ScatterContentSpawnBlockerEntityData>
	{
		public new FrostySdk.Ebx.ScatterContentSpawnBlockerEntityData Data => data as FrostySdk.Ebx.ScatterContentSpawnBlockerEntityData;
		public override string DisplayName => "ScatterContentSpawnBlocker";

		public ScatterContentSpawnBlockerEntity(FrostySdk.Ebx.ScatterContentSpawnBlockerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

