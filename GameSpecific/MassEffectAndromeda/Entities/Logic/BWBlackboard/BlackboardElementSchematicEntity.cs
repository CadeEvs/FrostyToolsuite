using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BlackboardElementSchematicEntityData))]
	public class BlackboardElementSchematicEntity : LogicEntity, IEntityData<FrostySdk.Ebx.BlackboardElementSchematicEntityData>
	{
		public new FrostySdk.Ebx.BlackboardElementSchematicEntityData Data => data as FrostySdk.Ebx.BlackboardElementSchematicEntityData;
		public override string DisplayName => "BlackboardElementSchematic";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Links
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Context", Direction.In),
				new ConnectionDesc("Entity", Direction.In)
			};
        }
        public override IEnumerable<ConnectionDesc> Events
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Execute", Direction.In)
			};
        }

        public BlackboardElementSchematicEntity(FrostySdk.Ebx.BlackboardElementSchematicEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

