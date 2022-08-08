using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PreloadTextureEntityData))]
	public class PreloadTextureEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PreloadTextureEntityData>
	{
		public new FrostySdk.Ebx.PreloadTextureEntityData Data => data as FrostySdk.Ebx.PreloadTextureEntityData;
		public override string DisplayName => "PreloadTexture";

		public PreloadTextureEntity(FrostySdk.Ebx.PreloadTextureEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

