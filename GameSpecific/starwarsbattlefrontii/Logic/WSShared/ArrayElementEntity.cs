using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ArrayElementEntityData))]
	public class ArrayElementEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ArrayElementEntityData>
	{
		public new FrostySdk.Ebx.ArrayElementEntityData Data => data as FrostySdk.Ebx.ArrayElementEntityData;
		public override string DisplayName => "ArrayElement";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ArrayElementEntity(FrostySdk.Ebx.ArrayElementEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

