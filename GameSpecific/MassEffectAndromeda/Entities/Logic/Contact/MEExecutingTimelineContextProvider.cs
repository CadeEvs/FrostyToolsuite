using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEExecutingTimelineContextProviderData))]
	public class MEExecutingTimelineContextProvider : ContextProvider, IEntityData<FrostySdk.Ebx.MEExecutingTimelineContextProviderData>
	{
		public new FrostySdk.Ebx.MEExecutingTimelineContextProviderData Data => data as FrostySdk.Ebx.MEExecutingTimelineContextProviderData;
		public override string DisplayName => "MEExecutingTimelineContextProvider";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Links
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Owner", Direction.In)
			};
		}

        public MEExecutingTimelineContextProvider(FrostySdk.Ebx.MEExecutingTimelineContextProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

