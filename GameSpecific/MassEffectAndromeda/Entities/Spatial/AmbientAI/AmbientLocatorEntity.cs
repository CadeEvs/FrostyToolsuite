using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AmbientLocatorEntityData))]
	public class AmbientLocatorEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.AmbientLocatorEntityData>
	{
		public new FrostySdk.Ebx.AmbientLocatorEntityData Data => data as FrostySdk.Ebx.AmbientLocatorEntityData;

		public AmbientLocatorEntity(FrostySdk.Ebx.AmbientLocatorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

