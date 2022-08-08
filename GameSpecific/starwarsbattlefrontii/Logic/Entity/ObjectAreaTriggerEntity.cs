using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ObjectAreaTriggerEntityData))]
	public class ObjectAreaTriggerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ObjectAreaTriggerEntityData>
	{
		public new FrostySdk.Ebx.ObjectAreaTriggerEntityData Data => data as FrostySdk.Ebx.ObjectAreaTriggerEntityData;
		public override string DisplayName => "ObjectAreaTrigger";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ObjectAreaTriggerEntity(FrostySdk.Ebx.ObjectAreaTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

