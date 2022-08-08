using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ControllerSpecificActionEntityData))]
	public class ControllerSpecificActionEntity : ControllerActionEntity, IEntityData<FrostySdk.Ebx.ControllerSpecificActionEntityData>
	{
		public new FrostySdk.Ebx.ControllerSpecificActionEntityData Data => data as FrostySdk.Ebx.ControllerSpecificActionEntityData;
		public override string DisplayName => "ControllerSpecificAction";

		public ControllerSpecificActionEntity(FrostySdk.Ebx.ControllerSpecificActionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

