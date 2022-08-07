using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ReadArrayElementEntityData))]
	public class ReadArrayElementEntity : ReadVariableBaseEntity, IEntityData<FrostySdk.Ebx.ReadArrayElementEntityData>
	{
		public new FrostySdk.Ebx.ReadArrayElementEntityData Data => data as FrostySdk.Ebx.ReadArrayElementEntityData;
		public override string DisplayName => "ReadArrayElement";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ReadArrayElementEntity(FrostySdk.Ebx.ReadArrayElementEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

