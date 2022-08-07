using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ReportInstallationProgressEntityData))]
	public class ReportInstallationProgressEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ReportInstallationProgressEntityData>
	{
		public new FrostySdk.Ebx.ReportInstallationProgressEntityData Data => data as FrostySdk.Ebx.ReportInstallationProgressEntityData;
		public override string DisplayName => "ReportInstallationProgress";
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Installing", Direction.Out)
			};
		}

		public ReportInstallationProgressEntity(FrostySdk.Ebx.ReportInstallationProgressEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

