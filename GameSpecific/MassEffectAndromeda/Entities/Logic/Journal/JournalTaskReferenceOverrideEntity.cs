using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.JournalTaskReferenceOverrideEntityData))]
	public class JournalTaskReferenceOverrideEntity : LogicEntity, IEntityData<FrostySdk.Ebx.JournalTaskReferenceOverrideEntityData>
	{
		public new FrostySdk.Ebx.JournalTaskReferenceOverrideEntityData Data => data as FrostySdk.Ebx.JournalTaskReferenceOverrideEntityData;
		public override string DisplayName => "JournalTaskReferenceOverride";

		public JournalTaskReferenceOverrideEntity(FrostySdk.Ebx.JournalTaskReferenceOverrideEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

