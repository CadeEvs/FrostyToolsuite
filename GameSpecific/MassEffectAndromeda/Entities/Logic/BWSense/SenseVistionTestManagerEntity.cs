using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SenseVistionTestManagerEntityData))]
	public class SenseVistionTestManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SenseVistionTestManagerEntityData>
	{
		public new FrostySdk.Ebx.SenseVistionTestManagerEntityData Data => data as FrostySdk.Ebx.SenseVistionTestManagerEntityData;
		public override string DisplayName => "SenseVistionTestManager";

		public SenseVistionTestManagerEntity(FrostySdk.Ebx.SenseVistionTestManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

