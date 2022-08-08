using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSMaterialVFXEntityData))]
	public class WSMaterialVFXEntity : MaterialBasedEffectEntity, IEntityData<FrostySdk.Ebx.WSMaterialVFXEntityData>
	{
		public new FrostySdk.Ebx.WSMaterialVFXEntityData Data => data as FrostySdk.Ebx.WSMaterialVFXEntityData;
		public override string DisplayName => "WSMaterialVFX";

		public WSMaterialVFXEntity(FrostySdk.Ebx.WSMaterialVFXEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

