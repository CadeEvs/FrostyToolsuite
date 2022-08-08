using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GetNameFromPlayerEventData))]
	public class GetNameFromPlayerEvent : LogicEntity, IEntityData<FrostySdk.Ebx.GetNameFromPlayerEventData>
	{
		public new FrostySdk.Ebx.GetNameFromPlayerEventData Data => data as FrostySdk.Ebx.GetNameFromPlayerEventData;
		public override string DisplayName => "GetNameFromPlayerEvent";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("OnlineId", Direction.Out)
			};
		}

		public GetNameFromPlayerEvent(FrostySdk.Ebx.GetNameFromPlayerEventData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

