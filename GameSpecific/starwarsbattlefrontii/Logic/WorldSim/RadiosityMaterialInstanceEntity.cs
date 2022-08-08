using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RadiosityMaterialInstanceEntityData))]
	public class RadiosityMaterialInstanceEntity : LogicEntity, IEntityData<FrostySdk.Ebx.RadiosityMaterialInstanceEntityData>
	{
		public new FrostySdk.Ebx.RadiosityMaterialInstanceEntityData Data => data as FrostySdk.Ebx.RadiosityMaterialInstanceEntityData;
		public override string DisplayName => "RadiosityMaterialInstance";

		public RadiosityMaterialInstanceEntity(FrostySdk.Ebx.RadiosityMaterialInstanceEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

