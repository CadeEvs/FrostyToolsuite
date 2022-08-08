using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HumanPlayerProxyEntityData))]
	public class HumanPlayerProxyEntity : LogicEntity, IEntityData<FrostySdk.Ebx.HumanPlayerProxyEntityData>
	{
		public new FrostySdk.Ebx.HumanPlayerProxyEntityData Data => data as FrostySdk.Ebx.HumanPlayerProxyEntityData;
		public override string DisplayName => "HumanPlayerProxy";

		public HumanPlayerProxyEntity(FrostySdk.Ebx.HumanPlayerProxyEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

