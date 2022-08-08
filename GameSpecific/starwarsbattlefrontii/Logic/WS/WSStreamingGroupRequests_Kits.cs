using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSStreamingGroupRequests_KitsData))]
	public class WSStreamingGroupRequests_Kits : WSSpawnBundleGroupRequestsStreamingBaseEntity, IEntityData<FrostySdk.Ebx.WSStreamingGroupRequests_KitsData>
	{
		public new FrostySdk.Ebx.WSStreamingGroupRequests_KitsData Data => data as FrostySdk.Ebx.WSStreamingGroupRequests_KitsData;
		public override string DisplayName => "WSStreamingGroupRequests_Kits";

		public WSStreamingGroupRequests_Kits(FrostySdk.Ebx.WSStreamingGroupRequests_KitsData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

