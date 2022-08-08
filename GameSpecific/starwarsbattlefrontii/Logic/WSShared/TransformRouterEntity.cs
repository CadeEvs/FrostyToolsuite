using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TransformRouterEntityData))]
	public class TransformRouterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TransformRouterEntityData>
	{
		public new FrostySdk.Ebx.TransformRouterEntityData Data => data as FrostySdk.Ebx.TransformRouterEntityData;
		public override string DisplayName => "TransformRouter";

		public TransformRouterEntity(FrostySdk.Ebx.TransformRouterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

