using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MESenseVisionTestManagerEntityData))]
	public class MESenseVisionTestManagerEntity : SenseVistionTestManagerEntity, IEntityData<FrostySdk.Ebx.MESenseVisionTestManagerEntityData>
	{
		public new FrostySdk.Ebx.MESenseVisionTestManagerEntityData Data => data as FrostySdk.Ebx.MESenseVisionTestManagerEntityData;
		public override string DisplayName => "MESenseVisionTestManager";

		public MESenseVisionTestManagerEntity(FrostySdk.Ebx.MESenseVisionTestManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

