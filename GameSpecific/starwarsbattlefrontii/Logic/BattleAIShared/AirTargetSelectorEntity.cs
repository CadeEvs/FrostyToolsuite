using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AirTargetSelectorEntityData))]
	public class AirTargetSelectorEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AirTargetSelectorEntityData>
	{
		public new FrostySdk.Ebx.AirTargetSelectorEntityData Data => data as FrostySdk.Ebx.AirTargetSelectorEntityData;
		public override string DisplayName => "AirTargetSelector";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public AirTargetSelectorEntity(FrostySdk.Ebx.AirTargetSelectorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

