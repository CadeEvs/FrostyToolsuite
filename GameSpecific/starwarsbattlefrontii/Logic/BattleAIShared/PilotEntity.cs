using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PilotEntityData))]
	public class PilotEntity : PilotEntityBase, IEntityData<FrostySdk.Ebx.PilotEntityData>
	{
		public new FrostySdk.Ebx.PilotEntityData Data => data as FrostySdk.Ebx.PilotEntityData;
		public override string DisplayName => "Pilot";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PilotEntity(FrostySdk.Ebx.PilotEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

