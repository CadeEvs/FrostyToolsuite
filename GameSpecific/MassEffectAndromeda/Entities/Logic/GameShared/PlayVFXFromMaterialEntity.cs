using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayVFXFromMaterialEntityData))]
	public class PlayVFXFromMaterialEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayVFXFromMaterialEntityData>
	{
		public new FrostySdk.Ebx.PlayVFXFromMaterialEntityData Data => data as FrostySdk.Ebx.PlayVFXFromMaterialEntityData;
		public override string DisplayName => "PlayVFXFromMaterial";

		public PlayVFXFromMaterialEntity(FrostySdk.Ebx.PlayVFXFromMaterialEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

