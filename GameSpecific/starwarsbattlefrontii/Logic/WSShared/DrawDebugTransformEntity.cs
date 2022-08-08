using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DrawDebugTransformEntityData))]
	public class DrawDebugTransformEntity : DrawDebugBaseEntity, IEntityData<FrostySdk.Ebx.DrawDebugTransformEntityData>
	{
		public new FrostySdk.Ebx.DrawDebugTransformEntityData Data => data as FrostySdk.Ebx.DrawDebugTransformEntityData;
		public override string DisplayName => "DrawDebugTransform";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DrawDebugTransformEntity(FrostySdk.Ebx.DrawDebugTransformEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

