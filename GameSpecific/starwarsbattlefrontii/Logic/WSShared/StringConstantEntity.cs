using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StringConstantEntityData))]
	public class StringConstantEntity : LogicEntity, IEntityData<FrostySdk.Ebx.StringConstantEntityData>
	{
		public new FrostySdk.Ebx.StringConstantEntityData Data => data as FrostySdk.Ebx.StringConstantEntityData;
		public override string DisplayName => "StringConstant";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public StringConstantEntity(FrostySdk.Ebx.StringConstantEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

