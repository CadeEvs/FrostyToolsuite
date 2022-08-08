using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LogitechLEDInputConceptIdentifierEntityData))]
	public class LogitechLEDInputConceptIdentifierEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LogitechLEDInputConceptIdentifierEntityData>
	{
		public new FrostySdk.Ebx.LogitechLEDInputConceptIdentifierEntityData Data => data as FrostySdk.Ebx.LogitechLEDInputConceptIdentifierEntityData;
		public override string DisplayName => "LogitechLEDInputConceptIdentifier";

		public LogitechLEDInputConceptIdentifierEntity(FrostySdk.Ebx.LogitechLEDInputConceptIdentifierEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

