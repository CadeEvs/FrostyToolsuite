using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DrawDebugCapsuleEntityData))]
	public class DrawDebugCapsuleEntity : DrawDebugBaseEntity, IEntityData<FrostySdk.Ebx.DrawDebugCapsuleEntityData>
	{
		public new FrostySdk.Ebx.DrawDebugCapsuleEntityData Data => data as FrostySdk.Ebx.DrawDebugCapsuleEntityData;
		public override string DisplayName => "DrawDebugCapsule";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DrawDebugCapsuleEntity(FrostySdk.Ebx.DrawDebugCapsuleEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

