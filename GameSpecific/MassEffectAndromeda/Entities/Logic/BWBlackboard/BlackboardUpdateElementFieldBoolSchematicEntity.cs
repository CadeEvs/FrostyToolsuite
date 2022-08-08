using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BlackboardUpdateElementFieldBoolSchematicEntityData))]
	public class BlackboardUpdateElementFieldBoolSchematicEntity : BlackboardUpdateElementFieldSchematicEntity, IEntityData<FrostySdk.Ebx.BlackboardUpdateElementFieldBoolSchematicEntityData>
	{
		public new FrostySdk.Ebx.BlackboardUpdateElementFieldBoolSchematicEntityData Data => data as FrostySdk.Ebx.BlackboardUpdateElementFieldBoolSchematicEntityData;
		public override string DisplayName => "BlackboardUpdateElementFieldBoolSchematic";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Value", Direction.In)
			};
		}

		public BlackboardUpdateElementFieldBoolSchematicEntity(FrostySdk.Ebx.BlackboardUpdateElementFieldBoolSchematicEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

