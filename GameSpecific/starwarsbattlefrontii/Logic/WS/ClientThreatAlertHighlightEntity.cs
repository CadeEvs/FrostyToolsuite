using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientThreatAlertHighlightEntityData))]
	public class ClientThreatAlertHighlightEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientThreatAlertHighlightEntityData>
	{
		public new FrostySdk.Ebx.ClientThreatAlertHighlightEntityData Data => data as FrostySdk.Ebx.ClientThreatAlertHighlightEntityData;
		public override string DisplayName => "ClientThreatAlertHighlight";

		public ClientThreatAlertHighlightEntity(FrostySdk.Ebx.ClientThreatAlertHighlightEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

