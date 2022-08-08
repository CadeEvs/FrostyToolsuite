using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DestinationDataInfoEntityData))]
	public class DestinationDataInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DestinationDataInfoEntityData>
	{
		public new FrostySdk.Ebx.DestinationDataInfoEntityData Data => data as FrostySdk.Ebx.DestinationDataInfoEntityData;
		public override string DisplayName => "DestinationDataInfo";

		public DestinationDataInfoEntity(FrostySdk.Ebx.DestinationDataInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

