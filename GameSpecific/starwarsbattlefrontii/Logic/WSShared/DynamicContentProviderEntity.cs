using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DynamicContentProviderEntityData))]
	public class DynamicContentProviderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DynamicContentProviderEntityData>
	{
		public new FrostySdk.Ebx.DynamicContentProviderEntityData Data => data as FrostySdk.Ebx.DynamicContentProviderEntityData;
		public override string DisplayName => "DynamicContentProvider";

		public DynamicContentProviderEntity(FrostySdk.Ebx.DynamicContentProviderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

