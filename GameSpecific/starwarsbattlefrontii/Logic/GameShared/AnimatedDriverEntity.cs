using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AnimatedDriverEntityData))]
	public class AnimatedDriverEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AnimatedDriverEntityData>
	{
		public new FrostySdk.Ebx.AnimatedDriverEntityData Data => data as FrostySdk.Ebx.AnimatedDriverEntityData;
		public override string DisplayName => "AnimatedDriver";

		public AnimatedDriverEntity(FrostySdk.Ebx.AnimatedDriverEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

