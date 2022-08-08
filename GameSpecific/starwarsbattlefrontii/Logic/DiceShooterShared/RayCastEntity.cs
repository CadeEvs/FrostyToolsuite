using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RayCastEntityData))]
	public class RayCastEntity : LogicEntity, IEntityData<FrostySdk.Ebx.RayCastEntityData>
	{
		public new FrostySdk.Ebx.RayCastEntityData Data => data as FrostySdk.Ebx.RayCastEntityData;
		public override string DisplayName => "RayCast";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public RayCastEntity(FrostySdk.Ebx.RayCastEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

