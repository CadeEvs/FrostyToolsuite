using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DistributedOrInputEntityData))]
	public class DistributedOrInputEntity : DistributedOrEntity, IEntityData<FrostySdk.Ebx.DistributedOrInputEntityData>
	{
		public new FrostySdk.Ebx.DistributedOrInputEntityData Data => data as FrostySdk.Ebx.DistributedOrInputEntityData;
		public override string DisplayName => "DistributedOrInput";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DistributedOrInputEntity(FrostySdk.Ebx.DistributedOrInputEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

