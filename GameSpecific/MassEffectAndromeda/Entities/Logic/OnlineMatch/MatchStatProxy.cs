using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MatchStatProxyData))]
	public class MatchStatProxy : LogicEntity, IEntityData<FrostySdk.Ebx.MatchStatProxyData>
	{
		public new FrostySdk.Ebx.MatchStatProxyData Data => data as FrostySdk.Ebx.MatchStatProxyData;
		public override string DisplayName => "MatchStatProxy";

		public MatchStatProxy(FrostySdk.Ebx.MatchStatProxyData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

