using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RadiosityModifierEntityData))]
	public class RadiosityModifierEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.RadiosityModifierEntityData>
	{
		public new FrostySdk.Ebx.RadiosityModifierEntityData Data => data as FrostySdk.Ebx.RadiosityModifierEntityData;

		public RadiosityModifierEntity(FrostySdk.Ebx.RadiosityModifierEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

