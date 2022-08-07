using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MultilineStringEntityData))]
	public class MultilineStringEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MultilineStringEntityData>
	{
		public new FrostySdk.Ebx.MultilineStringEntityData Data => data as FrostySdk.Ebx.MultilineStringEntityData;
		public override string DisplayName => "MultilineString";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public MultilineStringEntity(FrostySdk.Ebx.MultilineStringEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

