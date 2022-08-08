using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterPoseEntityData))]
	public class CharacterPoseEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CharacterPoseEntityData>
	{
		public new FrostySdk.Ebx.CharacterPoseEntityData Data => data as FrostySdk.Ebx.CharacterPoseEntityData;
		public override string DisplayName => "CharacterPose";

		public CharacterPoseEntity(FrostySdk.Ebx.CharacterPoseEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

