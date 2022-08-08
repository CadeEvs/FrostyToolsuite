using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ControllerActionEntityData))]
	public class ControllerActionEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ControllerActionEntityData>
	{
		public new FrostySdk.Ebx.ControllerActionEntityData Data => data as FrostySdk.Ebx.ControllerActionEntityData;
		public override string DisplayName => "ControllerAction";

		public ControllerActionEntity(FrostySdk.Ebx.ControllerActionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

