using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ProjectileStateEntityData))]
	public class ProjectileStateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ProjectileStateEntityData>
	{
		public new FrostySdk.Ebx.ProjectileStateEntityData Data => data as FrostySdk.Ebx.ProjectileStateEntityData;
		public override string DisplayName => "ProjectileState";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ProjectileStateEntity(FrostySdk.Ebx.ProjectileStateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

