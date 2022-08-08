using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSHumanPlayerProxyEntityData))]
	public class WSHumanPlayerProxyEntity : HumanPlayerProxyEntity, IEntityData<FrostySdk.Ebx.WSHumanPlayerProxyEntityData>
	{
		public new FrostySdk.Ebx.WSHumanPlayerProxyEntityData Data => data as FrostySdk.Ebx.WSHumanPlayerProxyEntityData;
		public override string DisplayName => "WSHumanPlayerProxy";

		public WSHumanPlayerProxyEntity(FrostySdk.Ebx.WSHumanPlayerProxyEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

