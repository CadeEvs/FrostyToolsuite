using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpaceDestinationRequestEntityData))]
	public class SpaceDestinationRequestEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SpaceDestinationRequestEntityData>
	{
		public new FrostySdk.Ebx.SpaceDestinationRequestEntityData Data => data as FrostySdk.Ebx.SpaceDestinationRequestEntityData;
		public override string DisplayName => "SpaceDestinationRequest";

		public SpaceDestinationRequestEntity(FrostySdk.Ebx.SpaceDestinationRequestEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

