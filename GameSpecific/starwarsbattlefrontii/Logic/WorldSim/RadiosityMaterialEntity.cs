using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RadiosityMaterialEntityData))]
	public class RadiosityMaterialEntity : LogicEntity, IEntityData<FrostySdk.Ebx.RadiosityMaterialEntityData>
	{
		public new FrostySdk.Ebx.RadiosityMaterialEntityData Data => data as FrostySdk.Ebx.RadiosityMaterialEntityData;
		public override string DisplayName => "RadiosityMaterial";

		public RadiosityMaterialEntity(FrostySdk.Ebx.RadiosityMaterialEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

