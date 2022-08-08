using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSPrintDebugTextEntityData))]
	public class WSPrintDebugTextEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WSPrintDebugTextEntityData>
	{
		public new FrostySdk.Ebx.WSPrintDebugTextEntityData Data => data as FrostySdk.Ebx.WSPrintDebugTextEntityData;
		public override string DisplayName => "WSPrintDebugText";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public WSPrintDebugTextEntity(FrostySdk.Ebx.WSPrintDebugTextEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

