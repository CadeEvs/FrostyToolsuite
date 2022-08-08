using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RadiosityMaterialOpacityTriggerEntityData))]
	public class RadiosityMaterialOpacityTriggerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.RadiosityMaterialOpacityTriggerEntityData>
	{
		public new FrostySdk.Ebx.RadiosityMaterialOpacityTriggerEntityData Data => data as FrostySdk.Ebx.RadiosityMaterialOpacityTriggerEntityData;
		public override string DisplayName => "RadiosityMaterialOpacityTrigger";

		public RadiosityMaterialOpacityTriggerEntity(FrostySdk.Ebx.RadiosityMaterialOpacityTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

