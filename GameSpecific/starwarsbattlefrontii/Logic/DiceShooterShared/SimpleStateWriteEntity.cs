using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SimpleStateWriteEntityData))]
	public class SimpleStateWriteEntity : SimpleStateEntityBase, IEntityData<FrostySdk.Ebx.SimpleStateWriteEntityData>
	{
		public new FrostySdk.Ebx.SimpleStateWriteEntityData Data => data as FrostySdk.Ebx.SimpleStateWriteEntityData;
		public override string DisplayName => "SimpleStateWrite";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SimpleStateWriteEntity(FrostySdk.Ebx.SimpleStateWriteEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

