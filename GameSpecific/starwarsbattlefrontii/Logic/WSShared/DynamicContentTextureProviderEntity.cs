using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DynamicContentTextureProviderEntityData))]
	public class DynamicContentTextureProviderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DynamicContentTextureProviderEntityData>
	{
		public new FrostySdk.Ebx.DynamicContentTextureProviderEntityData Data => data as FrostySdk.Ebx.DynamicContentTextureProviderEntityData;
		public override string DisplayName => "DynamicContentTextureProvider";

		public DynamicContentTextureProviderEntity(FrostySdk.Ebx.DynamicContentTextureProviderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

