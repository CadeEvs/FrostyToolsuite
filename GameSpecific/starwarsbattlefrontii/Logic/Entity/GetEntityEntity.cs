using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GetEntityEntityData))]
	public class GetEntityEntity : LogicEntity, IEntityData<FrostySdk.Ebx.GetEntityEntityData>
	{
		public new FrostySdk.Ebx.GetEntityEntityData Data => data as FrostySdk.Ebx.GetEntityEntityData;
		public override string DisplayName => "GetEntity";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public GetEntityEntity(FrostySdk.Ebx.GetEntityEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

