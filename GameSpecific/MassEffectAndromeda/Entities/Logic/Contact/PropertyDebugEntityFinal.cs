using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PropertyDebugEntityFinalData))]
	public class PropertyDebugEntityFinal : LogicEntity, IEntityData<FrostySdk.Ebx.PropertyDebugEntityFinalData>
	{
		public new FrostySdk.Ebx.PropertyDebugEntityFinalData Data => data as FrostySdk.Ebx.PropertyDebugEntityFinalData;
		public override string DisplayName => "PropertyDebugEntityFinal";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PropertyDebugEntityFinal(FrostySdk.Ebx.PropertyDebugEntityFinalData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

