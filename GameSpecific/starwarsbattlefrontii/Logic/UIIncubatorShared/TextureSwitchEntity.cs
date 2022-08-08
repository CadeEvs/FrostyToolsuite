using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TextureSwitchEntityData))]
	public class TextureSwitchEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TextureSwitchEntityData>
	{
		public new FrostySdk.Ebx.TextureSwitchEntityData Data => data as FrostySdk.Ebx.TextureSwitchEntityData;
		public override string DisplayName => "TextureSwitch";

		public TextureSwitchEntity(FrostySdk.Ebx.TextureSwitchEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

