using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PropertyDebugEntityData))]
	public class PropertyDebugEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PropertyDebugEntityData>
	{
		public new FrostySdk.Ebx.PropertyDebugEntityData Data => data as FrostySdk.Ebx.PropertyDebugEntityData;
		public override string DisplayName => "PropertyDebug";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PropertyDebugEntity(FrostySdk.Ebx.PropertyDebugEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

