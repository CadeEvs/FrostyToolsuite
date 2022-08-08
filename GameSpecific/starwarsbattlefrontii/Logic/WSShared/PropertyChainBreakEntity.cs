using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PropertyChainBreakEntityData))]
	public class PropertyChainBreakEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PropertyChainBreakEntityData>
	{
		public new FrostySdk.Ebx.PropertyChainBreakEntityData Data => data as FrostySdk.Ebx.PropertyChainBreakEntityData;
		public override string DisplayName => "PropertyChainBreak";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PropertyChainBreakEntity(FrostySdk.Ebx.PropertyChainBreakEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

