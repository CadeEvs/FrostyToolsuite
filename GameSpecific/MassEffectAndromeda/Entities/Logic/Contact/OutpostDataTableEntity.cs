using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OutpostDataTableEntityData))]
	public class OutpostDataTableEntity : OutpostDataTable, IEntityData<FrostySdk.Ebx.OutpostDataTableEntityData>
	{
		public new FrostySdk.Ebx.OutpostDataTableEntityData Data => data as FrostySdk.Ebx.OutpostDataTableEntityData;
		public override string DisplayName => "OutpostDataTable";

		public OutpostDataTableEntity(FrostySdk.Ebx.OutpostDataTableEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

