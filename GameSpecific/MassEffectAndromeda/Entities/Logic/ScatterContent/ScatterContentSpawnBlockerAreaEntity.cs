using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ScatterContentSpawnBlockerAreaEntityData))]
	public class ScatterContentSpawnBlockerAreaEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ScatterContentSpawnBlockerAreaEntityData>
	{
		public new FrostySdk.Ebx.ScatterContentSpawnBlockerAreaEntityData Data => data as FrostySdk.Ebx.ScatterContentSpawnBlockerAreaEntityData;
		public override string DisplayName => "ScatterContentSpawnBlockerArea";

		public ScatterContentSpawnBlockerAreaEntity(FrostySdk.Ebx.ScatterContentSpawnBlockerAreaEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

