using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BlackboardGetElementVector3SchematicEntityData))]
	public class BlackboardGetElementVector3SchematicEntity : BlackboardGetElementSchematicEntity, IEntityData<FrostySdk.Ebx.BlackboardGetElementVector3SchematicEntityData>
	{
		public new FrostySdk.Ebx.BlackboardGetElementVector3SchematicEntityData Data => data as FrostySdk.Ebx.BlackboardGetElementVector3SchematicEntityData;
		public override string DisplayName => "BlackboardGetElementVector3Schematic";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public BlackboardGetElementVector3SchematicEntity(FrostySdk.Ebx.BlackboardGetElementVector3SchematicEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

