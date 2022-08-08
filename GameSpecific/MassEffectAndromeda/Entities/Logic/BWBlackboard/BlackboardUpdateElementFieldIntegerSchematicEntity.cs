using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BlackboardUpdateElementFieldIntegerSchematicEntityData))]
	public class BlackboardUpdateElementFieldIntegerSchematicEntity : BlackboardUpdateElementFieldSchematicEntity, IEntityData<FrostySdk.Ebx.BlackboardUpdateElementFieldIntegerSchematicEntityData>
	{
		public new FrostySdk.Ebx.BlackboardUpdateElementFieldIntegerSchematicEntityData Data => data as FrostySdk.Ebx.BlackboardUpdateElementFieldIntegerSchematicEntityData;
		public override string DisplayName => "BlackboardUpdateElementFieldIntegerSchematic";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public BlackboardUpdateElementFieldIntegerSchematicEntity(FrostySdk.Ebx.BlackboardUpdateElementFieldIntegerSchematicEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

