using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharGenVisibilityEntityData))]
	public class CharGenVisibilityEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CharGenVisibilityEntityData>
	{
		public new FrostySdk.Ebx.CharGenVisibilityEntityData Data => data as FrostySdk.Ebx.CharGenVisibilityEntityData;
		public override string DisplayName => "CharGenVisibility";

		public CharGenVisibilityEntity(FrostySdk.Ebx.CharGenVisibilityEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

