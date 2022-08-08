using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BlackboardUpdateElementFieldVector3SchematicEntityData))]
	public class BlackboardUpdateElementFieldVector3SchematicEntity : BlackboardUpdateElementFieldSchematicEntity, IEntityData<FrostySdk.Ebx.BlackboardUpdateElementFieldVector3SchematicEntityData>
	{
		public new FrostySdk.Ebx.BlackboardUpdateElementFieldVector3SchematicEntityData Data => data as FrostySdk.Ebx.BlackboardUpdateElementFieldVector3SchematicEntityData;
		public override string DisplayName => "BlackboardUpdateElementFieldVector3Schematic";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public BlackboardUpdateElementFieldVector3SchematicEntity(FrostySdk.Ebx.BlackboardUpdateElementFieldVector3SchematicEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

