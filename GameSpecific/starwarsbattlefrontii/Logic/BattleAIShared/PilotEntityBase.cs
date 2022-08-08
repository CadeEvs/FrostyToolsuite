using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PilotEntityBaseData))]
	public class PilotEntityBase : LogicEntity, IEntityData<FrostySdk.Ebx.PilotEntityBaseData>
	{
		public new FrostySdk.Ebx.PilotEntityBaseData Data => data as FrostySdk.Ebx.PilotEntityBaseData;
		public override string DisplayName => "PilotEntityBase";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PilotEntityBase(FrostySdk.Ebx.PilotEntityBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

