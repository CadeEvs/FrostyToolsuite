using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSCorpseModifierEntityData))]
	public class WSCorpseModifierEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WSCorpseModifierEntityData>
	{
		public new FrostySdk.Ebx.WSCorpseModifierEntityData Data => data as FrostySdk.Ebx.WSCorpseModifierEntityData;
		public override string DisplayName => "WSCorpseModifier";

		public WSCorpseModifierEntity(FrostySdk.Ebx.WSCorpseModifierEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

