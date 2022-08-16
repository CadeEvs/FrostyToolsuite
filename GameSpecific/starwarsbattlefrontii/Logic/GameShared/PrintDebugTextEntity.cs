using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PrintDebugTextEntityData))]
	public class PrintDebugTextEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PrintDebugTextEntityData>
	{
		public new FrostySdk.Ebx.PrintDebugTextEntityData Data => data as FrostySdk.Ebx.PrintDebugTextEntityData;
		public override string DisplayName => "PrintDebugText";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Print", Direction.In)
			};
		}
		
		public override IEnumerable<string> HeaderRows
		{
			get
			{
				List<string> outHeaderRows = new List<string>();
				if (Data.Text != "")
				{
					outHeaderRows.Add($"Text: {Data.Text}");
				}
				return outHeaderRows;
			}
		}
		
		public PrintDebugTextEntity(FrostySdk.Ebx.PrintDebugTextEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

