using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SelectEventEntityData))]
	public class SelectEventEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SelectEventEntityData>
	{
		public new FrostySdk.Ebx.SelectEventEntityData Data => data as FrostySdk.Ebx.SelectEventEntityData;
		public override string DisplayName => "SelectEvent";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Events
		{
			get
			{
				List<ConnectionDesc> outEvents = new List<ConnectionDesc>();
				outEvents.Add(new ConnectionDesc("In", Direction.In));
				for (int i = 0; i < Data.Events.Count; i++)
				{
					outEvents.Add(new ConnectionDesc() { Name = Data.Events[i], Direction = Direction.Out });
				}
				return outEvents;
			}
		}
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Selected", Direction.In)
			};
		}

		public SelectEventEntity(FrostySdk.Ebx.SelectEventEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

