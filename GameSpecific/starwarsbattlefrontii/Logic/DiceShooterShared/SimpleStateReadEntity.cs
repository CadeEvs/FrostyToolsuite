using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SimpleStateReadEntityData))]
	public class SimpleStateReadEntity : SimpleStateEntityBase, IEntityData<FrostySdk.Ebx.SimpleStateReadEntityData>
	{
		public new FrostySdk.Ebx.SimpleStateReadEntityData Data => data as FrostySdk.Ebx.SimpleStateReadEntityData;
		public override string DisplayName => "SimpleStateRead";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SimpleStateReadEntity(FrostySdk.Ebx.SimpleStateReadEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

