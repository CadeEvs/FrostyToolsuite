using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CountDownEntityData))]
	public class CountDownEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CountDownEntityData>
	{
		public new FrostySdk.Ebx.CountDownEntityData Data => data as FrostySdk.Ebx.CountDownEntityData;
		public override string DisplayName => "CountDown";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CountDownEntity(FrostySdk.Ebx.CountDownEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

