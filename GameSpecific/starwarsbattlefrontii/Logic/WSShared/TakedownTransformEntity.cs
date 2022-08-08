using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TakedownTransformEntityData))]
	public class TakedownTransformEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TakedownTransformEntityData>
	{
		public new FrostySdk.Ebx.TakedownTransformEntityData Data => data as FrostySdk.Ebx.TakedownTransformEntityData;
		public override string DisplayName => "TakedownTransform";

		public TakedownTransformEntity(FrostySdk.Ebx.TakedownTransformEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

