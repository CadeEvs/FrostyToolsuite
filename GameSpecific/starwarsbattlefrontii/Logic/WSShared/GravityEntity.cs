using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GravityEntityData))]
	public class GravityEntity : LogicEntity, IEntityData<FrostySdk.Ebx.GravityEntityData>
	{
		public new FrostySdk.Ebx.GravityEntityData Data => data as FrostySdk.Ebx.GravityEntityData;
		public override string DisplayName => "Gravity";

		public GravityEntity(FrostySdk.Ebx.GravityEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

