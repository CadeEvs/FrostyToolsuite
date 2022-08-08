using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.IsNavMeshUnderEntityData))]
	public class IsNavMeshUnderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.IsNavMeshUnderEntityData>
	{
		public new FrostySdk.Ebx.IsNavMeshUnderEntityData Data => data as FrostySdk.Ebx.IsNavMeshUnderEntityData;
		public override string DisplayName => "IsNavMeshUnder";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public IsNavMeshUnderEntity(FrostySdk.Ebx.IsNavMeshUnderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

