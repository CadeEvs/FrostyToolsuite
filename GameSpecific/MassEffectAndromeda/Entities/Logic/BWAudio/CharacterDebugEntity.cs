using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterDebugEntityData))]
	public class CharacterDebugEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CharacterDebugEntityData>
	{
		public new FrostySdk.Ebx.CharacterDebugEntityData Data => data as FrostySdk.Ebx.CharacterDebugEntityData;
		public override string DisplayName => "CharacterDebug";

		public CharacterDebugEntity(FrostySdk.Ebx.CharacterDebugEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

