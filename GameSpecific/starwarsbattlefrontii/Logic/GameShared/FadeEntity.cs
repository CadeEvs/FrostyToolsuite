using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FadeEntityData))]
	public class FadeEntity : LogicEntity, IEntityData<FrostySdk.Ebx.FadeEntityData>
	{
		public new FrostySdk.Ebx.FadeEntityData Data => data as FrostySdk.Ebx.FadeEntityData;
		public override string DisplayName => "Fade";

		public FadeEntity(FrostySdk.Ebx.FadeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

