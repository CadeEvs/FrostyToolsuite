using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BlackboardGetElementEntitySchematicEntityData))]
	public class BlackboardGetElementEntitySchematicEntity : BlackboardGetElementSchematicEntity, IEntityData<FrostySdk.Ebx.BlackboardGetElementEntitySchematicEntityData>
	{
		public new FrostySdk.Ebx.BlackboardGetElementEntitySchematicEntityData Data => data as FrostySdk.Ebx.BlackboardGetElementEntitySchematicEntityData;
		public override string DisplayName => "BlackboardGetElementEntitySchematic";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public BlackboardGetElementEntitySchematicEntity(FrostySdk.Ebx.BlackboardGetElementEntitySchematicEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

