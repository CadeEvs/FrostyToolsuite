using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.KillswitchEntityData))]
	public class KillswitchEntity : LogicEntity, IEntityData<FrostySdk.Ebx.KillswitchEntityData>
	{
		public new FrostySdk.Ebx.KillswitchEntityData Data => data as FrostySdk.Ebx.KillswitchEntityData;
		public override string DisplayName => "Killswitch";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public KillswitchEntity(FrostySdk.Ebx.KillswitchEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

