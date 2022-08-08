using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MemoryPoolControlEntityData))]
	public class MemoryPoolControlEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MemoryPoolControlEntityData>
	{
		public new FrostySdk.Ebx.MemoryPoolControlEntityData Data => data as FrostySdk.Ebx.MemoryPoolControlEntityData;
		public override string DisplayName => "MemoryPoolControl";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public MemoryPoolControlEntity(FrostySdk.Ebx.MemoryPoolControlEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

