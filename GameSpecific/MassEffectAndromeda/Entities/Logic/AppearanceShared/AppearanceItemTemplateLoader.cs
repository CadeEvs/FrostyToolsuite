using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AppearanceItemTemplateLoaderData))]
	public class AppearanceItemTemplateLoader : LogicEntity, IEntityData<FrostySdk.Ebx.AppearanceItemTemplateLoaderData>
	{
		public new FrostySdk.Ebx.AppearanceItemTemplateLoaderData Data => data as FrostySdk.Ebx.AppearanceItemTemplateLoaderData;
		public override string DisplayName => "AppearanceItemTemplateLoader";

		public AppearanceItemTemplateLoader(FrostySdk.Ebx.AppearanceItemTemplateLoaderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

