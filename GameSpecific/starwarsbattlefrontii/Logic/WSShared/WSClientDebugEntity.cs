using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSClientDebugEntityData))]
	public class WSClientDebugEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WSClientDebugEntityData>
	{
		public new FrostySdk.Ebx.WSClientDebugEntityData Data => data as FrostySdk.Ebx.WSClientDebugEntityData;
		public override string DisplayName => "WSClientDebug";

		public WSClientDebugEntity(FrostySdk.Ebx.WSClientDebugEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

