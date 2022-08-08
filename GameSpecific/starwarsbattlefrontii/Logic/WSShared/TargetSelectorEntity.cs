using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TargetSelectorEntityData))]
	public class TargetSelectorEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TargetSelectorEntityData>
	{
		public new FrostySdk.Ebx.TargetSelectorEntityData Data => data as FrostySdk.Ebx.TargetSelectorEntityData;
		public override string DisplayName => "TargetSelector";

		public TargetSelectorEntity(FrostySdk.Ebx.TargetSelectorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

