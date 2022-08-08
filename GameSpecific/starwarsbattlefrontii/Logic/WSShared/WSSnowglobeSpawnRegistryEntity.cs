using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSSnowglobeSpawnRegistryEntityData))]
	public class WSSnowglobeSpawnRegistryEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WSSnowglobeSpawnRegistryEntityData>
	{
		public new FrostySdk.Ebx.WSSnowglobeSpawnRegistryEntityData Data => data as FrostySdk.Ebx.WSSnowglobeSpawnRegistryEntityData;
		public override string DisplayName => "WSSnowglobeSpawnRegistry";

		public WSSnowglobeSpawnRegistryEntity(FrostySdk.Ebx.WSSnowglobeSpawnRegistryEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

