using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TargetInfoData))]
	public class TargetInfo : LogicEntity, IEntityData<FrostySdk.Ebx.TargetInfoData>
	{
		public new FrostySdk.Ebx.TargetInfoData Data => data as FrostySdk.Ebx.TargetInfoData;
		public override string DisplayName => "TargetInfo";

		public TargetInfo(FrostySdk.Ebx.TargetInfoData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

