using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HonorAwardInfoEntityData))]
	public class HonorAwardInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.HonorAwardInfoEntityData>
	{
		public new FrostySdk.Ebx.HonorAwardInfoEntityData Data => data as FrostySdk.Ebx.HonorAwardInfoEntityData;
		public override string DisplayName => "HonorAwardInfo";

		public HonorAwardInfoEntity(FrostySdk.Ebx.HonorAwardInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

