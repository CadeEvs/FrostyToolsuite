using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DisplayInKillLogEntityData))]
	public class DisplayInKillLogEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DisplayInKillLogEntityData>
	{
		public new FrostySdk.Ebx.DisplayInKillLogEntityData Data => data as FrostySdk.Ebx.DisplayInKillLogEntityData;
		public override string DisplayName => "DisplayInKillLog";

		public DisplayInKillLogEntity(FrostySdk.Ebx.DisplayInKillLogEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

