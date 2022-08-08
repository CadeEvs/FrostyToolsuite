using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DataFilterEntityData))]
	public class DataFilterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DataFilterEntityData>
	{
		public new FrostySdk.Ebx.DataFilterEntityData Data => data as FrostySdk.Ebx.DataFilterEntityData;
		public override string DisplayName => "DataFilter";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DataFilterEntity(FrostySdk.Ebx.DataFilterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

