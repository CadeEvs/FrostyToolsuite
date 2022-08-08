using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PA2BehaviorEntityData))]
	public class PA2BehaviorEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PA2BehaviorEntityData>
	{
		public new FrostySdk.Ebx.PA2BehaviorEntityData Data => data as FrostySdk.Ebx.PA2BehaviorEntityData;
		public override string DisplayName => "PA2Behavior";

		public PA2BehaviorEntity(FrostySdk.Ebx.PA2BehaviorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

