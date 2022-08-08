using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CustomPlotSchematicCallbackEntityData))]
	public class CustomPlotSchematicCallbackEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CustomPlotSchematicCallbackEntityData>
	{
		public new FrostySdk.Ebx.CustomPlotSchematicCallbackEntityData Data => data as FrostySdk.Ebx.CustomPlotSchematicCallbackEntityData;
		public override string DisplayName => "CustomPlotSchematicCallback";

		public CustomPlotSchematicCallbackEntity(FrostySdk.Ebx.CustomPlotSchematicCallbackEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

