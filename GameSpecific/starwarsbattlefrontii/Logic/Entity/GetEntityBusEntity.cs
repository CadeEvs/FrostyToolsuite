using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GetEntityBusEntityData))]
	public class GetEntityBusEntity : LogicEntity, IEntityData<FrostySdk.Ebx.GetEntityBusEntityData>
	{
		public new FrostySdk.Ebx.GetEntityBusEntityData Data => data as FrostySdk.Ebx.GetEntityBusEntityData;
		public override string DisplayName => "GetEntityBus";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public GetEntityBusEntity(FrostySdk.Ebx.GetEntityBusEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

