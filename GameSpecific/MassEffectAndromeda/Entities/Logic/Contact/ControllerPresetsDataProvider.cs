using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ControllerPresetsDataProviderData))]
	public class ControllerPresetsDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.ControllerPresetsDataProviderData>
	{
		public new FrostySdk.Ebx.ControllerPresetsDataProviderData Data => data as FrostySdk.Ebx.ControllerPresetsDataProviderData;
		public override string DisplayName => "ControllerPresetsDataProvider";

		public ControllerPresetsDataProvider(FrostySdk.Ebx.ControllerPresetsDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

