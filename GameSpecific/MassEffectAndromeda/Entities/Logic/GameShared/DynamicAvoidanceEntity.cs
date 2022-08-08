using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DynamicAvoidanceEntityData))]
	public class DynamicAvoidanceEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DynamicAvoidanceEntityData>
	{
		public new FrostySdk.Ebx.DynamicAvoidanceEntityData Data => data as FrostySdk.Ebx.DynamicAvoidanceEntityData;
		public override string DisplayName => "DynamicAvoidance";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DynamicAvoidanceEntity(FrostySdk.Ebx.DynamicAvoidanceEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

