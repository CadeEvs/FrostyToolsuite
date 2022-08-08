using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FormatStringEntityData))]
	public class FormatStringEntity : LogicEntity, IEntityData<FrostySdk.Ebx.FormatStringEntityData>
	{
		public new FrostySdk.Ebx.FormatStringEntityData Data => data as FrostySdk.Ebx.FormatStringEntityData;
		public override string DisplayName => "FormatString";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public FormatStringEntity(FrostySdk.Ebx.FormatStringEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

