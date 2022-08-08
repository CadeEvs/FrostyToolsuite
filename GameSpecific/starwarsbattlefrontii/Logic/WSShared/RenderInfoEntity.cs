using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RenderInfoEntityData))]
	public class RenderInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.RenderInfoEntityData>
	{
		public new FrostySdk.Ebx.RenderInfoEntityData Data => data as FrostySdk.Ebx.RenderInfoEntityData;
		public override string DisplayName => "RenderInfo";

		public RenderInfoEntity(FrostySdk.Ebx.RenderInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

