using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BlackboardAddElementEntitySchematicEntityData))]
	public class BlackboardAddElementEntitySchematicEntity : BlackboardElementSchematicEntity, IEntityData<FrostySdk.Ebx.BlackboardAddElementEntitySchematicEntityData>
	{
		public new FrostySdk.Ebx.BlackboardAddElementEntitySchematicEntityData Data => data as FrostySdk.Ebx.BlackboardAddElementEntitySchematicEntityData;
		public override string DisplayName => "BlackboardAddElementEntitySchematic";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public BlackboardAddElementEntitySchematicEntity(FrostySdk.Ebx.BlackboardAddElementEntitySchematicEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

