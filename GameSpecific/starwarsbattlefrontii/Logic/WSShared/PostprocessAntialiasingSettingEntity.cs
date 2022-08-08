using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PostprocessAntialiasingSettingEntityData))]
	public class PostprocessAntialiasingSettingEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PostprocessAntialiasingSettingEntityData>
	{
		public new FrostySdk.Ebx.PostprocessAntialiasingSettingEntityData Data => data as FrostySdk.Ebx.PostprocessAntialiasingSettingEntityData;
		public override string DisplayName => "PostprocessAntialiasingSetting";

		public PostprocessAntialiasingSettingEntity(FrostySdk.Ebx.PostprocessAntialiasingSettingEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

