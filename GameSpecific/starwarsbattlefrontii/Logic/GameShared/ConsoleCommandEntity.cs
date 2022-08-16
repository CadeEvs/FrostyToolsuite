using System.Collections.Generic;
using FrostySdk.Ebx;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ConsoleCommandEntityData))]
	public class ConsoleCommandEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ConsoleCommandEntityData>
	{
		public new FrostySdk.Ebx.ConsoleCommandEntityData Data => data as FrostySdk.Ebx.ConsoleCommandEntityData;
		public override string DisplayName => "ConsoleCommand";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Execute", Direction.In)
			};
		}
		
		public override IEnumerable<string> HeaderRows
		{
			get
			{
				List<string> outHeaderRows = new List<string>();
				foreach (CString command in Data.Commands)
				{
					outHeaderRows.Add($"Command: {command}");
				}
				return outHeaderRows;
			}
		}
		
		public ConsoleCommandEntity(FrostySdk.Ebx.ConsoleCommandEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

