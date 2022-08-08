using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OutpostDataTableProxyEntityData))]
	public class OutpostDataTableProxyEntity : OutpostDataTable, IEntityData<FrostySdk.Ebx.OutpostDataTableProxyEntityData>
	{
		public new FrostySdk.Ebx.OutpostDataTableProxyEntityData Data => data as FrostySdk.Ebx.OutpostDataTableProxyEntityData;
		public override string DisplayName => "OutpostDataTableProxy";

		public OutpostDataTableProxyEntity(FrostySdk.Ebx.OutpostDataTableProxyEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

