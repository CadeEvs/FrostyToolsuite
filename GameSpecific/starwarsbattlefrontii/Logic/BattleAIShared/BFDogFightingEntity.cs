using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BFDogFightingEntityData))]
	public class BFDogFightingEntity : DogFightingEntity, IEntityData<FrostySdk.Ebx.BFDogFightingEntityData>
	{
		public new FrostySdk.Ebx.BFDogFightingEntityData Data => data as FrostySdk.Ebx.BFDogFightingEntityData;
		public override string DisplayName => "BFDogFighting";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public BFDogFightingEntity(FrostySdk.Ebx.BFDogFightingEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

