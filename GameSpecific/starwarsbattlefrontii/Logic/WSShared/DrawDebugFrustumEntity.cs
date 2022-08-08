using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DrawDebugFrustumEntityData))]
	public class DrawDebugFrustumEntity : DrawDebugBaseEntity, IEntityData<FrostySdk.Ebx.DrawDebugFrustumEntityData>
	{
		public new FrostySdk.Ebx.DrawDebugFrustumEntityData Data => data as FrostySdk.Ebx.DrawDebugFrustumEntityData;
		public override string DisplayName => "DrawDebugFrustum";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DrawDebugFrustumEntity(FrostySdk.Ebx.DrawDebugFrustumEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

