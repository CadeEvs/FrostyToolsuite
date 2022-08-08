using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WorldToLocalTransformEntityData))]
	public class WorldToLocalTransformEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WorldToLocalTransformEntityData>
	{
		public new FrostySdk.Ebx.WorldToLocalTransformEntityData Data => data as FrostySdk.Ebx.WorldToLocalTransformEntityData;
		public override string DisplayName => "WorldToLocalTransform";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public WorldToLocalTransformEntity(FrostySdk.Ebx.WorldToLocalTransformEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

