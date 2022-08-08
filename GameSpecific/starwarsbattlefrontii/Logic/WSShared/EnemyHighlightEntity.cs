using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EnemyHighlightEntityData))]
	public class EnemyHighlightEntity : LogicEntity, IEntityData<FrostySdk.Ebx.EnemyHighlightEntityData>
	{
		public new FrostySdk.Ebx.EnemyHighlightEntityData Data => data as FrostySdk.Ebx.EnemyHighlightEntityData;
		public override string DisplayName => "EnemyHighlight";

		public EnemyHighlightEntity(FrostySdk.Ebx.EnemyHighlightEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

