using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BlackboardGetElementBoolSchematicEntityData))]
	public class BlackboardGetElementBoolSchematicEntity : BlackboardGetElementSchematicEntity, IEntityData<FrostySdk.Ebx.BlackboardGetElementBoolSchematicEntityData>
	{
		public new FrostySdk.Ebx.BlackboardGetElementBoolSchematicEntityData Data => data as FrostySdk.Ebx.BlackboardGetElementBoolSchematicEntityData;
		public override string DisplayName => "BlackboardGetElementBoolSchematic";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public BlackboardGetElementBoolSchematicEntity(FrostySdk.Ebx.BlackboardGetElementBoolSchematicEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

