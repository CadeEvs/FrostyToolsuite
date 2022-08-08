using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AntRigCacheEntityData))]
	public class AntRigCacheEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AntRigCacheEntityData>
	{
		public new FrostySdk.Ebx.AntRigCacheEntityData Data => data as FrostySdk.Ebx.AntRigCacheEntityData;
		public override string DisplayName => "AntRigCache";

		public AntRigCacheEntity(FrostySdk.Ebx.AntRigCacheEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

