using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ExpressionEntityData))]
	public class ExpressionEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ExpressionEntityData>
	{
		public new FrostySdk.Ebx.ExpressionEntityData Data => data as FrostySdk.Ebx.ExpressionEntityData;
		public override string DisplayName => "Expression";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ExpressionEntity(FrostySdk.Ebx.ExpressionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

