using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TacticalSpawnQueryEntityData))]
	public class TacticalSpawnQueryEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TacticalSpawnQueryEntityData>
	{
		public new FrostySdk.Ebx.TacticalSpawnQueryEntityData Data => data as FrostySdk.Ebx.TacticalSpawnQueryEntityData;
		public override string DisplayName => "TacticalSpawnQuery";

		public TacticalSpawnQueryEntity(FrostySdk.Ebx.TacticalSpawnQueryEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

