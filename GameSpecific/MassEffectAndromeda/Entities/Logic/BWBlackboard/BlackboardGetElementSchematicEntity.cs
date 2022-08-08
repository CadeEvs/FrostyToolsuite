using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BlackboardGetElementSchematicEntityData))]
	public class BlackboardGetElementSchematicEntity : BlackboardElementSchematicEntity, IEntityData<FrostySdk.Ebx.BlackboardGetElementSchematicEntityData>
	{
		public new FrostySdk.Ebx.BlackboardGetElementSchematicEntityData Data => data as FrostySdk.Ebx.BlackboardGetElementSchematicEntityData;
		public override string DisplayName => "BlackboardGetElementSchematic";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public BlackboardGetElementSchematicEntity(FrostySdk.Ebx.BlackboardGetElementSchematicEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

