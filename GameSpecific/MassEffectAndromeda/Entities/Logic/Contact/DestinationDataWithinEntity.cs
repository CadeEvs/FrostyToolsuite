using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DestinationDataWithinEntityData))]
	public class DestinationDataWithinEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DestinationDataWithinEntityData>
	{
		public new FrostySdk.Ebx.DestinationDataWithinEntityData Data => data as FrostySdk.Ebx.DestinationDataWithinEntityData;
		public override string DisplayName => "DestinationDataWithin";

		public DestinationDataWithinEntity(FrostySdk.Ebx.DestinationDataWithinEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

