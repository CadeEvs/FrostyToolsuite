using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ScatterContentMultiCategoryControllerEntityData))]
	public class ScatterContentMultiCategoryControllerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ScatterContentMultiCategoryControllerEntityData>
	{
		public new FrostySdk.Ebx.ScatterContentMultiCategoryControllerEntityData Data => data as FrostySdk.Ebx.ScatterContentMultiCategoryControllerEntityData;
		public override string DisplayName => "ScatterContentMultiCategoryController";

		public ScatterContentMultiCategoryControllerEntity(FrostySdk.Ebx.ScatterContentMultiCategoryControllerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

