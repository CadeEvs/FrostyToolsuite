using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AurebeshOutputEntityData))]
	public class AurebeshOutputEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AurebeshOutputEntityData>
	{
		public new FrostySdk.Ebx.AurebeshOutputEntityData Data => data as FrostySdk.Ebx.AurebeshOutputEntityData;
		public override string DisplayName => "AurebeshOutput";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public AurebeshOutputEntity(FrostySdk.Ebx.AurebeshOutputEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

