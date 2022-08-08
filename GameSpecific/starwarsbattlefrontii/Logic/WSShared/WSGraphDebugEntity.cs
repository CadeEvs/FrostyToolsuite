using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSGraphDebugEntityData))]
	public class WSGraphDebugEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WSGraphDebugEntityData>
	{
		public new FrostySdk.Ebx.WSGraphDebugEntityData Data => data as FrostySdk.Ebx.WSGraphDebugEntityData;
		public override string DisplayName => "WSGraphDebug";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public WSGraphDebugEntity(FrostySdk.Ebx.WSGraphDebugEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

