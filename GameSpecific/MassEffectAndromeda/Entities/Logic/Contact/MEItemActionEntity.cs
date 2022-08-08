using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEItemActionEntityData))]
	public class MEItemActionEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MEItemActionEntityData>
	{
		public new FrostySdk.Ebx.MEItemActionEntityData Data => data as FrostySdk.Ebx.MEItemActionEntityData;
		public override string DisplayName => "MEItemAction";
        public override IEnumerable<ConnectionDesc> Links
        {
            get => new List<ConnectionDesc>()
            {
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

        public MEItemActionEntity(FrostySdk.Ebx.MEItemActionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

