using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ArraySizeEntityData))]
	public class ArraySizeEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ArraySizeEntityData>
	{
		public new FrostySdk.Ebx.ArraySizeEntityData Data => data as FrostySdk.Ebx.ArraySizeEntityData;
		public override string DisplayName => "ArraySize";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ArraySizeEntity(FrostySdk.Ebx.ArraySizeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

