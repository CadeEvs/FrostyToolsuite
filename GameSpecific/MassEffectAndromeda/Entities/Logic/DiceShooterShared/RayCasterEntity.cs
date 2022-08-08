using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RayCasterEntityData))]
	public class RayCasterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.RayCasterEntityData>
	{
		public new FrostySdk.Ebx.RayCasterEntityData Data => data as FrostySdk.Ebx.RayCasterEntityData;
		public override string DisplayName => "RayCaster";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public RayCasterEntity(FrostySdk.Ebx.RayCasterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

