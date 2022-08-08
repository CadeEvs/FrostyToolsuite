using FrostySdk;
using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.IntSwitchEventEntityData))]
	public class IntSwitchEventEntity : LogicEntity, IEntityData<FrostySdk.Ebx.IntSwitchEventEntityData>
	{
		public new FrostySdk.Ebx.IntSwitchEventEntityData Data => data as FrostySdk.Ebx.IntSwitchEventEntityData;
		public override string DisplayName => "IntSwitchEvent";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Events
		{
			get
			{
				List<ConnectionDesc> outEvents = new List<ConnectionDesc>();
				outEvents.Add(new ConnectionDesc("In", Direction.In));

				foreach (var comparisonRange in Data.ComparisonRanges)
				{
					outEvents.Add(new ConnectionDesc() { Name = Utils.GetString((int)comparisonRange.EventHash), Direction = Direction.Out });
				}

				return outEvents;
			}
		}
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Value", Direction.In)
			};
		}

		public IntSwitchEventEntity(FrostySdk.Ebx.IntSwitchEventEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

