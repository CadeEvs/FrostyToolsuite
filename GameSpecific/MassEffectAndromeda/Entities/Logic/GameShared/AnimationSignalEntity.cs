using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AnimationSignalEntityData))]
	public class AnimationSignalEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AnimationSignalEntityData>
	{
		public new FrostySdk.Ebx.AnimationSignalEntityData Data => data as FrostySdk.Ebx.AnimationSignalEntityData;
		public override string DisplayName => "AnimationSignal";

		public AnimationSignalEntity(FrostySdk.Ebx.AnimationSignalEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

