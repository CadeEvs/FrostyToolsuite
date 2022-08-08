using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GetEntityFromEventEntityData))]
	public class GetEntityFromEventEntity : LogicEntity, IEntityData<FrostySdk.Ebx.GetEntityFromEventEntityData>
	{
		public new FrostySdk.Ebx.GetEntityFromEventEntityData Data => data as FrostySdk.Ebx.GetEntityFromEventEntityData;
		public override string DisplayName => "GetEntityFromEvent";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Links
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Entity", Direction.Out)
			};
		}
        public override IEnumerable<ConnectionDesc> Events
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("In", Direction.In)
			};
		}

        public GetEntityFromEventEntity(FrostySdk.Ebx.GetEntityFromEventEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

