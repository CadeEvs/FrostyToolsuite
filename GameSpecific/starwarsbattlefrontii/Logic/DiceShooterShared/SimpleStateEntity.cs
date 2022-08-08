using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SimpleStateEntityData))]
	public class SimpleStateEntity : SimpleStateEntityBase, IEntityData<FrostySdk.Ebx.SimpleStateEntityData>
	{
		public new FrostySdk.Ebx.SimpleStateEntityData Data => data as FrostySdk.Ebx.SimpleStateEntityData;
		public override string DisplayName => "SimpleState";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SimpleStateEntity(FrostySdk.Ebx.SimpleStateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

