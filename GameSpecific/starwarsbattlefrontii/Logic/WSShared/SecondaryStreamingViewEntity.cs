using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SecondaryStreamingViewEntityData))]
	public class SecondaryStreamingViewEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SecondaryStreamingViewEntityData>
	{
		public new FrostySdk.Ebx.SecondaryStreamingViewEntityData Data => data as FrostySdk.Ebx.SecondaryStreamingViewEntityData;
		public override string DisplayName => "SecondaryStreamingView";

		public SecondaryStreamingViewEntity(FrostySdk.Ebx.SecondaryStreamingViewEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

