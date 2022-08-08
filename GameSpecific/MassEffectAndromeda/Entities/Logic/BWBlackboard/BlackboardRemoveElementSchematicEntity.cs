using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BlackboardRemoveElementSchematicEntityData))]
	public class BlackboardRemoveElementSchematicEntity : BlackboardElementSchematicEntity, IEntityData<FrostySdk.Ebx.BlackboardRemoveElementSchematicEntityData>
	{
		public new FrostySdk.Ebx.BlackboardRemoveElementSchematicEntityData Data => data as FrostySdk.Ebx.BlackboardRemoveElementSchematicEntityData;
		public override string DisplayName => "BlackboardRemoveElementSchematic";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public BlackboardRemoveElementSchematicEntity(FrostySdk.Ebx.BlackboardRemoveElementSchematicEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

