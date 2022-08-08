using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TacticalSpawnManagerEntityData))]
	public class TacticalSpawnManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TacticalSpawnManagerEntityData>
	{
		public new FrostySdk.Ebx.TacticalSpawnManagerEntityData Data => data as FrostySdk.Ebx.TacticalSpawnManagerEntityData;
		public override string DisplayName => "TacticalSpawnManager";

		public TacticalSpawnManagerEntity(FrostySdk.Ebx.TacticalSpawnManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

