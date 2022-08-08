using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StringBuilderEntityData))]
	public class StringBuilderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.StringBuilderEntityData>
	{
		public new FrostySdk.Ebx.StringBuilderEntityData Data => data as FrostySdk.Ebx.StringBuilderEntityData;
		public override string DisplayName => "StringBuilder";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public StringBuilderEntity(FrostySdk.Ebx.StringBuilderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

