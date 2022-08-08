using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ConsoleCommandEntityData))]
	public class ConsoleCommandEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ConsoleCommandEntityData>
	{
		public new FrostySdk.Ebx.ConsoleCommandEntityData Data => data as FrostySdk.Ebx.ConsoleCommandEntityData;
		public override string DisplayName => "ConsoleCommand";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ConsoleCommandEntity(FrostySdk.Ebx.ConsoleCommandEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

