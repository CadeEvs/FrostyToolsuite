using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LocalizedStringContainerEntityData))]
	public class LocalizedStringContainerEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.LocalizedStringContainerEntityData>
	{
		public new FrostySdk.Ebx.LocalizedStringContainerEntityData Data => data as FrostySdk.Ebx.LocalizedStringContainerEntityData;

		public LocalizedStringContainerEntity(FrostySdk.Ebx.LocalizedStringContainerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

