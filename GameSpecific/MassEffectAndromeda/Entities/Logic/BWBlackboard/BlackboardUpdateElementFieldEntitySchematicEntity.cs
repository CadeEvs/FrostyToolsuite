using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BlackboardUpdateElementFieldEntitySchematicEntityData))]
	public class BlackboardUpdateElementFieldEntitySchematicEntity : BlackboardUpdateElementFieldSchematicEntity, IEntityData<FrostySdk.Ebx.BlackboardUpdateElementFieldEntitySchematicEntityData>
	{
		public new FrostySdk.Ebx.BlackboardUpdateElementFieldEntitySchematicEntityData Data => data as FrostySdk.Ebx.BlackboardUpdateElementFieldEntitySchematicEntityData;
		public override string DisplayName => "BlackboardUpdateElementFieldEntitySchematic";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public BlackboardUpdateElementFieldEntitySchematicEntity(FrostySdk.Ebx.BlackboardUpdateElementFieldEntitySchematicEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

