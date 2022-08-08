using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CreatureLocoServerAuthEntityData))]
	public class CreatureLocoServerAuthEntity : CreatureLocoEntity, IEntityData<FrostySdk.Ebx.CreatureLocoServerAuthEntityData>
	{
		public new FrostySdk.Ebx.CreatureLocoServerAuthEntityData Data => data as FrostySdk.Ebx.CreatureLocoServerAuthEntityData;
		public override string DisplayName => "CreatureLocoServerAuth";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CreatureLocoServerAuthEntity(FrostySdk.Ebx.CreatureLocoServerAuthEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

