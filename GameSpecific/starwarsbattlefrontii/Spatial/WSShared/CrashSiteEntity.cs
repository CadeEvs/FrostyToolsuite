using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CrashSiteEntityData))]
	public class CrashSiteEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.CrashSiteEntityData>
	{
		public new FrostySdk.Ebx.CrashSiteEntityData Data => data as FrostySdk.Ebx.CrashSiteEntityData;
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CrashSiteEntity(FrostySdk.Ebx.CrashSiteEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

