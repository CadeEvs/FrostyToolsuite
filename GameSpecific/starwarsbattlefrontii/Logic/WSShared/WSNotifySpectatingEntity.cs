using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSNotifySpectatingEntityData))]
	public class WSNotifySpectatingEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WSNotifySpectatingEntityData>
	{
		public new FrostySdk.Ebx.WSNotifySpectatingEntityData Data => data as FrostySdk.Ebx.WSNotifySpectatingEntityData;
		public override string DisplayName => "WSNotifySpectating";

		public WSNotifySpectatingEntity(FrostySdk.Ebx.WSNotifySpectatingEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

