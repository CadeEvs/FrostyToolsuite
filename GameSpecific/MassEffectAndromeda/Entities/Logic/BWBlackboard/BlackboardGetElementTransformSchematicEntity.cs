using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BlackboardGetElementTransformSchematicEntityData))]
	public class BlackboardGetElementTransformSchematicEntity : BlackboardGetElementSchematicEntity, IEntityData<FrostySdk.Ebx.BlackboardGetElementTransformSchematicEntityData>
	{
		public new FrostySdk.Ebx.BlackboardGetElementTransformSchematicEntityData Data => data as FrostySdk.Ebx.BlackboardGetElementTransformSchematicEntityData;
		public override string DisplayName => "BlackboardGetElementTransformSchematic";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public BlackboardGetElementTransformSchematicEntity(FrostySdk.Ebx.BlackboardGetElementTransformSchematicEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

