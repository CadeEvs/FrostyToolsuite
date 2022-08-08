using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BlackboardUpdateElementFieldSchematicEntityData))]
	public class BlackboardUpdateElementFieldSchematicEntity : BlackboardElementSchematicEntity, IEntityData<FrostySdk.Ebx.BlackboardUpdateElementFieldSchematicEntityData>
	{
		public new FrostySdk.Ebx.BlackboardUpdateElementFieldSchematicEntityData Data => data as FrostySdk.Ebx.BlackboardUpdateElementFieldSchematicEntityData;
		public override string DisplayName => "BlackboardUpdateElementFieldSchematic";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public BlackboardUpdateElementFieldSchematicEntity(FrostySdk.Ebx.BlackboardUpdateElementFieldSchematicEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

