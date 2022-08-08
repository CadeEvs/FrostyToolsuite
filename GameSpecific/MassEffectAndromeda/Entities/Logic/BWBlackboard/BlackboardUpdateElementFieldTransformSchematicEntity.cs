using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BlackboardUpdateElementFieldTransformSchematicEntityData))]
	public class BlackboardUpdateElementFieldTransformSchematicEntity : BlackboardUpdateElementFieldSchematicEntity, IEntityData<FrostySdk.Ebx.BlackboardUpdateElementFieldTransformSchematicEntityData>
	{
		public new FrostySdk.Ebx.BlackboardUpdateElementFieldTransformSchematicEntityData Data => data as FrostySdk.Ebx.BlackboardUpdateElementFieldTransformSchematicEntityData;
		public override string DisplayName => "BlackboardUpdateElementFieldTransformSchematic";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Value", Direction.In)
			};
		}

		public BlackboardUpdateElementFieldTransformSchematicEntity(FrostySdk.Ebx.BlackboardUpdateElementFieldTransformSchematicEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

