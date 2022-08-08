using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TargetAngleFilterEntityData))]
	public class TargetAngleFilterEntity : TargetFilterEntity, IEntityData<FrostySdk.Ebx.TargetAngleFilterEntityData>
	{
		public new FrostySdk.Ebx.TargetAngleFilterEntityData Data => data as FrostySdk.Ebx.TargetAngleFilterEntityData;
		public override string DisplayName => "TargetAngleFilter";

		public TargetAngleFilterEntity(FrostySdk.Ebx.TargetAngleFilterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

