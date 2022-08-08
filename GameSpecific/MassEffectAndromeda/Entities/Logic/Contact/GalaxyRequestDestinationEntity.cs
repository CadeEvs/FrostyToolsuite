using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GalaxyRequestDestinationEntityData))]
	public class GalaxyRequestDestinationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.GalaxyRequestDestinationEntityData>
	{
		public new FrostySdk.Ebx.GalaxyRequestDestinationEntityData Data => data as FrostySdk.Ebx.GalaxyRequestDestinationEntityData;
		public override string DisplayName => "GalaxyRequestDestination";
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("CurrentDestination", Direction.Out),
				new ConnectionDesc("CurrentSystem", Direction.Out)
			};
		}

		public GalaxyRequestDestinationEntity(FrostySdk.Ebx.GalaxyRequestDestinationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

