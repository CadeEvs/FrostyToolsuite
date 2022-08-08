using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.IntersectionTriggerEntityData))]
	public class IntersectionTriggerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.IntersectionTriggerEntityData>
	{
		public new FrostySdk.Ebx.IntersectionTriggerEntityData Data => data as FrostySdk.Ebx.IntersectionTriggerEntityData;
		public override string DisplayName => "IntersectionTrigger";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public IntersectionTriggerEntity(FrostySdk.Ebx.IntersectionTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

