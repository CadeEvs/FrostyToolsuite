using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GameplayQueryBaseEntityData))]
	public class GameplayQueryBaseEntity : LogicEntity, IEntityData<FrostySdk.Ebx.GameplayQueryBaseEntityData>
	{
		public new FrostySdk.Ebx.GameplayQueryBaseEntityData Data => data as FrostySdk.Ebx.GameplayQueryBaseEntityData;
		public override string DisplayName => "GameplayQueryBase";

		public GameplayQueryBaseEntity(FrostySdk.Ebx.GameplayQueryBaseEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

