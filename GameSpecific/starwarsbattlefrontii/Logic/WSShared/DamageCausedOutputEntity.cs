using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DamageCausedOutputEntityData))]
	public class DamageCausedOutputEntity : DamageOutputEntity, IEntityData<FrostySdk.Ebx.DamageCausedOutputEntityData>
	{
		public new FrostySdk.Ebx.DamageCausedOutputEntityData Data => data as FrostySdk.Ebx.DamageCausedOutputEntityData;
		public override string DisplayName => "DamageCausedOutput";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DamageCausedOutputEntity(FrostySdk.Ebx.DamageCausedOutputEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

