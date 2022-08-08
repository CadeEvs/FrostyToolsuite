using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SyncedSequenceEntityData))]
	public class SyncedSequenceEntity : SequenceEntity, IEntityData<FrostySdk.Ebx.SyncedSequenceEntityData>
	{
		public new FrostySdk.Ebx.SyncedSequenceEntityData Data => data as FrostySdk.Ebx.SyncedSequenceEntityData;
		public override string DisplayName => "SyncedSequence";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SyncedSequenceEntity(FrostySdk.Ebx.SyncedSequenceEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

