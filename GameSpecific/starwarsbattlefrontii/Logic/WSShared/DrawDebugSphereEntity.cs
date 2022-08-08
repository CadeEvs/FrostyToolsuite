using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DrawDebugSphereEntityData))]
	public class DrawDebugSphereEntity : DrawDebugBaseEntity, IEntityData<FrostySdk.Ebx.DrawDebugSphereEntityData>
	{
		public new FrostySdk.Ebx.DrawDebugSphereEntityData Data => data as FrostySdk.Ebx.DrawDebugSphereEntityData;
		public override string DisplayName => "DrawDebugSphere";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DrawDebugSphereEntity(FrostySdk.Ebx.DrawDebugSphereEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

