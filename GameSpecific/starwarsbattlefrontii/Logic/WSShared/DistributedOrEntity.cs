using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DistributedOrEntityData))]
	public class DistributedOrEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DistributedOrEntityData>
	{
		public new FrostySdk.Ebx.DistributedOrEntityData Data => data as FrostySdk.Ebx.DistributedOrEntityData;
		public override string DisplayName => "DistributedOr";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DistributedOrEntity(FrostySdk.Ebx.DistributedOrEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

