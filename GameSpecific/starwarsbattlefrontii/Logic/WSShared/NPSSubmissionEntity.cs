using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.NPSSubmissionEntityData))]
	public class NPSSubmissionEntity : LogicEntity, IEntityData<FrostySdk.Ebx.NPSSubmissionEntityData>
	{
		public new FrostySdk.Ebx.NPSSubmissionEntityData Data => data as FrostySdk.Ebx.NPSSubmissionEntityData;
		public override string DisplayName => "NPSSubmission";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public NPSSubmissionEntity(FrostySdk.Ebx.NPSSubmissionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

