using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ValueMatchEntityData))]
	public class ValueMatchEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ValueMatchEntityData>
	{
		public new FrostySdk.Ebx.ValueMatchEntityData Data => data as FrostySdk.Ebx.ValueMatchEntityData;
		public override string DisplayName => "ValueMatch";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ValueMatchEntity(FrostySdk.Ebx.ValueMatchEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

