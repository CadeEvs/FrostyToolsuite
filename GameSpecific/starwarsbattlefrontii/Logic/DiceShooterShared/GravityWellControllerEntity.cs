using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GravityWellControllerEntityData))]
	public class GravityWellControllerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.GravityWellControllerEntityData>
	{
		public new FrostySdk.Ebx.GravityWellControllerEntityData Data => data as FrostySdk.Ebx.GravityWellControllerEntityData;
		public override string DisplayName => "GravityWellController";

		public GravityWellControllerEntity(FrostySdk.Ebx.GravityWellControllerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

