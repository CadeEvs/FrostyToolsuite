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
			get
			{
				List<string> outHeaderRows = new List<string>();
				if (Data.CommandName != "")
				{
					outHeaderRows.Add($"Command: {Data.CommandName}");
				}
				return outHeaderRows;
			}
		}
		
		public ConsoleCommandTriggerEntity(FrostySdk.Ebx.ConsoleCommandTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

