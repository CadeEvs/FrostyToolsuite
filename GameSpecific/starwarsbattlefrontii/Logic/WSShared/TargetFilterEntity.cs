using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TargetFilterEntityData))]
	public class TargetFilterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TargetFilterEntityData>
	{
		public new FrostySdk.Ebx.TargetFilterEntityData Data => data as FrostySdk.Ebx.TargetFilterEntityData;
		public override string DisplayName => "TargetFilter";

		public TargetFilterEntity(FrostySdk.Ebx.TargetFilterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

