using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ReadArraySizeEntityData))]
	public class ReadArraySizeEntity : ReadVariableBaseEntity, IEntityData<FrostySdk.Ebx.ReadArraySizeEntityData>
	{
		public new FrostySdk.Ebx.ReadArraySizeEntityData Data => data as FrostySdk.Ebx.ReadArraySizeEntityData;
		public override string DisplayName => "ReadArraySize";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ReadArraySizeEntity(FrostySdk.Ebx.ReadArraySizeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

