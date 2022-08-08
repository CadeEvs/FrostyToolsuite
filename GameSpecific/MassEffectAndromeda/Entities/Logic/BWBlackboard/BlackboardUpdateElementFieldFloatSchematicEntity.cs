using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BlackboardUpdateElementFieldFloatSchematicEntityData))]
	public class BlackboardUpdateElementFieldFloatSchematicEntity : BlackboardUpdateElementFieldSchematicEntity, IEntityData<FrostySdk.Ebx.BlackboardUpdateElementFieldFloatSchematicEntityData>
	{
		public new FrostySdk.Ebx.BlackboardUpdateElementFieldFloatSchematicEntityData Data => data as FrostySdk.Ebx.BlackboardUpdateElementFieldFloatSchematicEntityData;
		public override string DisplayName => "BlackboardUpdateElementFieldFloatSchematic";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Value", Direction.In)
			};
		}

		public BlackboardUpdateElementFieldFloatSchematicEntity(FrostySdk.Ebx.BlackboardUpdateElementFieldFloatSchematicEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

