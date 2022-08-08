using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BlackboardAddElementSchematicEntityData))]
	public class BlackboardAddElementSchematicEntity : BlackboardElementSchematicEntity, IEntityData<FrostySdk.Ebx.BlackboardAddElementSchematicEntityData>
	{
		public new FrostySdk.Ebx.BlackboardAddElementSchematicEntityData Data => data as FrostySdk.Ebx.BlackboardAddElementSchematicEntityData;
		public override string DisplayName => "BlackboardAddElementSchematic";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public BlackboardAddElementSchematicEntity(FrostySdk.Ebx.BlackboardAddElementSchematicEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

