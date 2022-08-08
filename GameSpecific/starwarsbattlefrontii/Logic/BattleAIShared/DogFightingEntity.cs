using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DogFightingEntityData))]
	public class DogFightingEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DogFightingEntityData>
	{
		public new FrostySdk.Ebx.DogFightingEntityData Data => data as FrostySdk.Ebx.DogFightingEntityData;
		public override string DisplayName => "DogFighting";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DogFightingEntity(FrostySdk.Ebx.DogFightingEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

