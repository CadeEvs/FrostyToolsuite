using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ScoreListenerEntityData))]
	public class ScoreListenerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ScoreListenerEntityData>
	{
		public new FrostySdk.Ebx.ScoreListenerEntityData Data => data as FrostySdk.Ebx.ScoreListenerEntityData;
		public override string DisplayName => "ScoreListener";
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Enabled", Direction.In)
			};
		}

		public ScoreListenerEntity(FrostySdk.Ebx.ScoreListenerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

