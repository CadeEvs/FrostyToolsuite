using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TargetTypeFilterEntityData))]
	public class TargetTypeFilterEntity : TargetFilterEntity, IEntityData<FrostySdk.Ebx.TargetTypeFilterEntityData>
	{
		public new FrostySdk.Ebx.TargetTypeFilterEntityData Data => data as FrostySdk.Ebx.TargetTypeFilterEntityData;
		public override string DisplayName => "TargetTypeFilter";

		public TargetTypeFilterEntity(FrostySdk.Ebx.TargetTypeFilterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

