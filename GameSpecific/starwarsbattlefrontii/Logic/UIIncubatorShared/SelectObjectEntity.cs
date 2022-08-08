using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SelectObjectEntityData))]
	public class SelectObjectEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SelectObjectEntityData>
	{
		public new FrostySdk.Ebx.SelectObjectEntityData Data => data as FrostySdk.Ebx.SelectObjectEntityData;
		public override string DisplayName => "SelectObject";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SelectObjectEntity(FrostySdk.Ebx.SelectObjectEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

