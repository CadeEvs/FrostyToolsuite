using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MilestonesDataTreeOutputEntityData))]
	public class MilestonesDataTreeOutputEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MilestonesDataTreeOutputEntityData>
	{
		public new FrostySdk.Ebx.MilestonesDataTreeOutputEntityData Data => data as FrostySdk.Ebx.MilestonesDataTreeOutputEntityData;
		public override string DisplayName => "MilestonesDataTreeOutput";

		public MilestonesDataTreeOutputEntity(FrostySdk.Ebx.MilestonesDataTreeOutputEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

