using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ConsoleCommandTriggerEntityData))]
	public class ConsoleCommandTriggerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ConsoleCommandTriggerEntityData>
	{
		public new FrostySdk.Ebx.ConsoleCommandTriggerEntityData Data => data as FrostySdk.Ebx.ConsoleCommandTriggerEntityData;
		public override string DisplayName => "ConsoleCommandTrigger";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("OnCommand", Direction.Out)
			};
		}
        public override IEnumerable<string> HeaderRows
        {
			get => new List<string>()
			{
				$"Command: {Data.CommandName}"
			};
        }

        public ConsoleCommandTriggerEntity(FrostySdk.Ebx.ConsoleCommandTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

