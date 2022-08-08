using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DistributedOrOutputEntityData))]
	public class DistributedOrOutputEntity : DistributedOrEntity, IEntityData<FrostySdk.Ebx.DistributedOrOutputEntityData>
	{
		public new FrostySdk.Ebx.DistributedOrOutputEntityData Data => data as FrostySdk.Ebx.DistributedOrOutputEntityData;
		public override string DisplayName => "DistributedOrOutput";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DistributedOrOutputEntity(FrostySdk.Ebx.DistributedOrOutputEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

