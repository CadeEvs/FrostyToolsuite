using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UpdateSpaceDestinationEntityData))]
	public class UpdateSpaceDestinationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UpdateSpaceDestinationEntityData>
	{
		public new FrostySdk.Ebx.UpdateSpaceDestinationEntityData Data => data as FrostySdk.Ebx.UpdateSpaceDestinationEntityData;
		public override string DisplayName => "UpdateSpaceDestination";

		public UpdateSpaceDestinationEntity(FrostySdk.Ebx.UpdateSpaceDestinationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

