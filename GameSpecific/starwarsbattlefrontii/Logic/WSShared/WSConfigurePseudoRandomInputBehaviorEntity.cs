using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSConfigurePseudoRandomInputBehaviorEntityData))]
	public class WSConfigurePseudoRandomInputBehaviorEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WSConfigurePseudoRandomInputBehaviorEntityData>
	{
		public new FrostySdk.Ebx.WSConfigurePseudoRandomInputBehaviorEntityData Data => data as FrostySdk.Ebx.WSConfigurePseudoRandomInputBehaviorEntityData;
		public override string DisplayName => "WSConfigurePseudoRandomInputBehavior";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public WSConfigurePseudoRandomInputBehaviorEntity(FrostySdk.Ebx.WSConfigurePseudoRandomInputBehaviorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

