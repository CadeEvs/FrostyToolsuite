using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CustomizeSoldierEntityData))]
	public class CustomizeSoldierEntity : CustomizeBaseEntity, IEntityData<FrostySdk.Ebx.CustomizeSoldierEntityData>
	{
		public new FrostySdk.Ebx.CustomizeSoldierEntityData Data => data as FrostySdk.Ebx.CustomizeSoldierEntityData;
		public override string DisplayName => "CustomizeSoldier";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CustomizeSoldierEntity(FrostySdk.Ebx.CustomizeSoldierEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

