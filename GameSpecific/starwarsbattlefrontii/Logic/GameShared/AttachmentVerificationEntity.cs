using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AttachmentVerificationEntityData))]
	public class AttachmentVerificationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AttachmentVerificationEntityData>
	{
		public new FrostySdk.Ebx.AttachmentVerificationEntityData Data => data as FrostySdk.Ebx.AttachmentVerificationEntityData;
		public override string DisplayName => "AttachmentVerification";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public AttachmentVerificationEntity(FrostySdk.Ebx.AttachmentVerificationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

