using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FlashGrenadeEntityData))]
	public class FlashGrenadeEntity : WSGrenadeEntity, IEntityData<FrostySdk.Ebx.FlashGrenadeEntityData>
	{
		public new FrostySdk.Ebx.FlashGrenadeEntityData Data => data as FrostySdk.Ebx.FlashGrenadeEntityData;

		public FlashGrenadeEntity(FrostySdk.Ebx.FlashGrenadeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

