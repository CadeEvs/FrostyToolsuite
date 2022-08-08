using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BlackboardGetElementIntegerSchematicEntityData))]
	public class BlackboardGetElementIntegerSchematicEntity : BlackboardGetElementSchematicEntity, IEntityData<FrostySdk.Ebx.BlackboardGetElementIntegerSchematicEntityData>
	{
		public new FrostySdk.Ebx.BlackboardGetElementIntegerSchematicEntityData Data => data as FrostySdk.Ebx.BlackboardGetElementIntegerSchematicEntityData;
		public override string DisplayName => "BlackboardGetElementIntegerSchematic";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public BlackboardGetElementIntegerSchematicEntity(FrostySdk.Ebx.BlackboardGetElementIntegerSchematicEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

