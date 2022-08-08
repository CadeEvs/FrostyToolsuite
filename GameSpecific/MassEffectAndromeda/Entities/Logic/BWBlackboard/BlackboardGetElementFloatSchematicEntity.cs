using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BlackboardGetElementFloatSchematicEntityData))]
	public class BlackboardGetElementFloatSchematicEntity : BlackboardGetElementSchematicEntity, IEntityData<FrostySdk.Ebx.BlackboardGetElementFloatSchematicEntityData>
	{
		public new FrostySdk.Ebx.BlackboardGetElementFloatSchematicEntityData Data => data as FrostySdk.Ebx.BlackboardGetElementFloatSchematicEntityData;
		public override string DisplayName => "BlackboardGetElementFloatSchematic";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Value", Direction.Out)
			};
		}

		public BlackboardGetElementFloatSchematicEntity(FrostySdk.Ebx.BlackboardGetElementFloatSchematicEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

