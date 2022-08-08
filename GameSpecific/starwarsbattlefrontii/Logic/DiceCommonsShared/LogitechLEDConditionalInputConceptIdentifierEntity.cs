using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LogitechLEDConditionalInputConceptIdentifierEntityData))]
	public class LogitechLEDConditionalInputConceptIdentifierEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LogitechLEDConditionalInputConceptIdentifierEntityData>
	{
		public new FrostySdk.Ebx.LogitechLEDConditionalInputConceptIdentifierEntityData Data => data as FrostySdk.Ebx.LogitechLEDConditionalInputConceptIdentifierEntityData;
		public override string DisplayName => "LogitechLEDConditionalInputConceptIdentifier";

		public LogitechLEDConditionalInputConceptIdentifierEntity(FrostySdk.Ebx.LogitechLEDConditionalInputConceptIdentifierEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

