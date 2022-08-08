using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ScreenControlEntityData))]
	public class ScreenControlEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ScreenControlEntityData>
	{
		public new FrostySdk.Ebx.ScreenControlEntityData Data => data as FrostySdk.Ebx.ScreenControlEntityData;
		public override string DisplayName => "ScreenControl";

		public ScreenControlEntity(FrostySdk.Ebx.ScreenControlEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

