using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEItemStackableCountEntityData))]
	public class MEItemStackableCountEntity : MEItemActionEntity, IEntityData<FrostySdk.Ebx.MEItemStackableCountEntityData>
	{
		public new FrostySdk.Ebx.MEItemStackableCountEntityData Data => data as FrostySdk.Ebx.MEItemStackableCountEntityData;
		public override string DisplayName => "MEItemStackableCount";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Events
        {
			get
            {
				List<ConnectionDesc> outEvents = new List<ConnectionDesc>();
				outEvents.AddRange(base.Events);
				outEvents.Add(new ConnectionDesc("OnChanged", Direction.Out));
				return outEvents;
            }
        }
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Count", Direction.Out)
			};
		}

		public MEItemStackableCountEntity(FrostySdk.Ebx.MEItemStackableCountEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

