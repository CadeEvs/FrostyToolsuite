using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BlackboardUpdateElementSchematicEntityData))]
	public class BlackboardUpdateElementSchematicEntity : BlackboardElementSchematicEntity, IEntityData<FrostySdk.Ebx.BlackboardUpdateElementSchematicEntityData>
	{
		public new FrostySdk.Ebx.BlackboardUpdateElementSchematicEntityData Data => data as FrostySdk.Ebx.BlackboardUpdateElementSchematicEntityData;
		public override string DisplayName => "BlackboardUpdateElementSchematic";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public BlackboardUpdateElementSchematicEntity(FrostySdk.Ebx.BlackboardUpdateElementSchematicEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

