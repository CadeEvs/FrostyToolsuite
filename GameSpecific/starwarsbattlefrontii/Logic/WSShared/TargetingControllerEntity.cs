using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TargetingControllerEntityData))]
	public class TargetingControllerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TargetingControllerEntityData>
	{
		public new FrostySdk.Ebx.TargetingControllerEntityData Data => data as FrostySdk.Ebx.TargetingControllerEntityData;
		public override string DisplayName => "TargetingController";

		public TargetingControllerEntity(FrostySdk.Ebx.TargetingControllerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

