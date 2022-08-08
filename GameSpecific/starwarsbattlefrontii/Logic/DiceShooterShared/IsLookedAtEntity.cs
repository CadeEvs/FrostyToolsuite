using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.IsLookedAtEntityData))]
	public class IsLookedAtEntity : LogicEntity, IEntityData<FrostySdk.Ebx.IsLookedAtEntityData>
	{
		public new FrostySdk.Ebx.IsLookedAtEntityData Data => data as FrostySdk.Ebx.IsLookedAtEntityData;
		public override string DisplayName => "IsLookedAt";

		public IsLookedAtEntity(FrostySdk.Ebx.IsLookedAtEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

