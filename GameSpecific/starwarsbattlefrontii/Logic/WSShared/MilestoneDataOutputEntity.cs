using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MilestoneDataOutputEntityData))]
	public class MilestoneDataOutputEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MilestoneDataOutputEntityData>
	{
		public new FrostySdk.Ebx.MilestoneDataOutputEntityData Data => data as FrostySdk.Ebx.MilestoneDataOutputEntityData;
		public override string DisplayName => "MilestoneDataOutput";

		public MilestoneDataOutputEntity(FrostySdk.Ebx.MilestoneDataOutputEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

