using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ConditionalTransformEntityData))]
	public class ConditionalTransformEntity : ConditionalStateEntity, IEntityData<FrostySdk.Ebx.ConditionalTransformEntityData>
	{
		public new FrostySdk.Ebx.ConditionalTransformEntityData Data => data as FrostySdk.Ebx.ConditionalTransformEntityData;
		public override string DisplayName => "ConditionalTransform";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ConditionalTransformEntity(FrostySdk.Ebx.ConditionalTransformEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

