using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BWApplyTimelineEntityBaseData))]
	public class BWApplyTimelineEntityBase : LogicEntity, IEntityData<FrostySdk.Ebx.BWApplyTimelineEntityBaseData>
	{
		public new FrostySdk.Ebx.BWApplyTimelineEntityBaseData Data => data as FrostySdk.Ebx.BWApplyTimelineEntityBaseData;
		public override string DisplayName => "BWApplyTimelineEntityBase";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Links
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Entities", Direction.In)
			};
		}

		public BWApplyTimelineEntityBase(FrostySdk.Ebx.BWApplyTimelineEntityBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

