using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VegetationBaseEntityData))]
	public class VegetationBaseEntity : GameComponentEntity, IEntityData<FrostySdk.Ebx.VegetationBaseEntityData>
	{
		public new FrostySdk.Ebx.VegetationBaseEntityData Data => data as FrostySdk.Ebx.VegetationBaseEntityData;

		public VegetationBaseEntity(FrostySdk.Ebx.VegetationBaseEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

