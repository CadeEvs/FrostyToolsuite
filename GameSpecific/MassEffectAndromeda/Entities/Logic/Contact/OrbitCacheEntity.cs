using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OrbitCacheEntityData))]
	public class OrbitCacheEntity : LogicEntity, IEntityData<FrostySdk.Ebx.OrbitCacheEntityData>
	{
		public new FrostySdk.Ebx.OrbitCacheEntityData Data => data as FrostySdk.Ebx.OrbitCacheEntityData;
		public override string DisplayName => "OrbitCache";

		public OrbitCacheEntity(FrostySdk.Ebx.OrbitCacheEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

