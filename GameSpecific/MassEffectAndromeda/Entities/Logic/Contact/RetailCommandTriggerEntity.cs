using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RetailCommandTriggerEntityData))]
	public class RetailCommandTriggerEntity : ConsoleCommandTriggerEntity, IEntityData<FrostySdk.Ebx.RetailCommandTriggerEntityData>
	{
		public new FrostySdk.Ebx.RetailCommandTriggerEntityData Data => data as FrostySdk.Ebx.RetailCommandTriggerEntityData;
		public override string DisplayName => "RetailCommandTrigger";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public RetailCommandTriggerEntity(FrostySdk.Ebx.RetailCommandTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

