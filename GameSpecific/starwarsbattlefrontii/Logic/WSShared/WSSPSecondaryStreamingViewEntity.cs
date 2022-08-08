using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSSPSecondaryStreamingViewEntityData))]
	public class WSSPSecondaryStreamingViewEntity : SecondaryStreamingViewEntity, IEntityData<FrostySdk.Ebx.WSSPSecondaryStreamingViewEntityData>
	{
		public new FrostySdk.Ebx.WSSPSecondaryStreamingViewEntityData Data => data as FrostySdk.Ebx.WSSPSecondaryStreamingViewEntityData;
		public override string DisplayName => "WSSPSecondaryStreamingView";

		public WSSPSecondaryStreamingViewEntity(FrostySdk.Ebx.WSSPSecondaryStreamingViewEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

