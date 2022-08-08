using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DynamicContentActionExecutorEntityData))]
	public class DynamicContentActionExecutorEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DynamicContentActionExecutorEntityData>
	{
		public new FrostySdk.Ebx.DynamicContentActionExecutorEntityData Data => data as FrostySdk.Ebx.DynamicContentActionExecutorEntityData;
		public override string DisplayName => "DynamicContentActionExecutor";

		public DynamicContentActionExecutorEntity(FrostySdk.Ebx.DynamicContentActionExecutorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

