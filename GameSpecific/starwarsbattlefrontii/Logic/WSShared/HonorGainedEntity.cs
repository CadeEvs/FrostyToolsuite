using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HonorGainedEntityData))]
	public class HonorGainedEntity : LogicEntity, IEntityData<FrostySdk.Ebx.HonorGainedEntityData>
	{
		public new FrostySdk.Ebx.HonorGainedEntityData Data => data as FrostySdk.Ebx.HonorGainedEntityData;
		public override string DisplayName => "HonorGained";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public HonorGainedEntity(FrostySdk.Ebx.HonorGainedEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

