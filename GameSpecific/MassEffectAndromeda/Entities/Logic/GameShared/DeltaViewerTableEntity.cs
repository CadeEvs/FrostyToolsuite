using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DeltaViewerTableEntityData))]
	public class DeltaViewerTableEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DeltaViewerTableEntityData>
	{
		public new FrostySdk.Ebx.DeltaViewerTableEntityData Data => data as FrostySdk.Ebx.DeltaViewerTableEntityData;
		public override string DisplayName => "DeltaViewerTable";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DeltaViewerTableEntity(FrostySdk.Ebx.DeltaViewerTableEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

