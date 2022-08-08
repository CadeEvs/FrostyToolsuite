using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LocalLocatorEntityData))]
	public class LocalLocatorEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.LocalLocatorEntityData>
	{
		public new FrostySdk.Ebx.LocalLocatorEntityData Data => data as FrostySdk.Ebx.LocalLocatorEntityData;
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public LocalLocatorEntity(FrostySdk.Ebx.LocalLocatorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

