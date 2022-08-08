using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSDogFightingEntityData))]
	public class WSDogFightingEntity : DogFightingEntity, IEntityData<FrostySdk.Ebx.WSDogFightingEntityData>
	{
		public new FrostySdk.Ebx.WSDogFightingEntityData Data => data as FrostySdk.Ebx.WSDogFightingEntityData;
		public override string DisplayName => "WSDogFighting";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public WSDogFightingEntity(FrostySdk.Ebx.WSDogFightingEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

