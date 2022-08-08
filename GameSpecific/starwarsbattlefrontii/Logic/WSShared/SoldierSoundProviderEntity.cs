using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoldierSoundProviderEntityData))]
	public class SoldierSoundProviderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SoldierSoundProviderEntityData>
	{
		public new FrostySdk.Ebx.SoldierSoundProviderEntityData Data => data as FrostySdk.Ebx.SoldierSoundProviderEntityData;
		public override string DisplayName => "SoldierSoundProvider";

		public SoldierSoundProviderEntity(FrostySdk.Ebx.SoldierSoundProviderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

