using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ImportExportOnlineAppearanceDataEntityData))]
	public class ImportExportOnlineAppearanceDataEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ImportExportOnlineAppearanceDataEntityData>
	{
		public new FrostySdk.Ebx.ImportExportOnlineAppearanceDataEntityData Data => data as FrostySdk.Ebx.ImportExportOnlineAppearanceDataEntityData;
		public override string DisplayName => "ImportExportOnlineAppearanceData";

		public ImportExportOnlineAppearanceDataEntity(FrostySdk.Ebx.ImportExportOnlineAppearanceDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

