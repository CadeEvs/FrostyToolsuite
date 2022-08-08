using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MilestonesFlattenedDataOutputEntityData))]
	public class MilestonesFlattenedDataOutputEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MilestonesFlattenedDataOutputEntityData>
	{
		public new FrostySdk.Ebx.MilestonesFlattenedDataOutputEntityData Data => data as FrostySdk.Ebx.MilestonesFlattenedDataOutputEntityData;
		public override string DisplayName => "MilestonesFlattenedDataOutput";

		public MilestonesFlattenedDataOutputEntity(FrostySdk.Ebx.MilestonesFlattenedDataOutputEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

