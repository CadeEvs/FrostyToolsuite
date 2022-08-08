using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BWAddStatElementData))]
	public class BWAddStatElement : LogicEntity, IEntityData<FrostySdk.Ebx.BWAddStatElementData>
	{
		public new FrostySdk.Ebx.BWAddStatElementData Data => data as FrostySdk.Ebx.BWAddStatElementData;
		public override string DisplayName => "BWAddStatElement";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public BWAddStatElement(FrostySdk.Ebx.BWAddStatElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

