using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CustomizeBaseEntityData))]
	public class CustomizeBaseEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CustomizeBaseEntityData>
	{
		public new FrostySdk.Ebx.CustomizeBaseEntityData Data => data as FrostySdk.Ebx.CustomizeBaseEntityData;
		public override string DisplayName => "CustomizeBase";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CustomizeBaseEntity(FrostySdk.Ebx.CustomizeBaseEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

