using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MPMatchDataProviderData))]
	public class MPMatchDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.MPMatchDataProviderData>
	{
		public new FrostySdk.Ebx.MPMatchDataProviderData Data => data as FrostySdk.Ebx.MPMatchDataProviderData;
		public override string DisplayName => "MPMatchDataProvider";
        public override IEnumerable<ConnectionDesc> Properties
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Latency", Direction.Out)
			};
		}

        public MPMatchDataProvider(FrostySdk.Ebx.MPMatchDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

