using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SPProgressionCollectibleEntityData))]
	public class SPProgressionCollectibleEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SPProgressionCollectibleEntityData>
	{
		public new FrostySdk.Ebx.SPProgressionCollectibleEntityData Data => data as FrostySdk.Ebx.SPProgressionCollectibleEntityData;
		public override string DisplayName => "SPProgressionCollectible";

		public SPProgressionCollectibleEntity(FrostySdk.Ebx.SPProgressionCollectibleEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

