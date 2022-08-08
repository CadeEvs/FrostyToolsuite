using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RoundOverEntityData))]
	public class RoundOverEntity : LogicEntity, IEntityData<FrostySdk.Ebx.RoundOverEntityData>
	{
		public new FrostySdk.Ebx.RoundOverEntityData Data => data as FrostySdk.Ebx.RoundOverEntityData;
		public override string DisplayName => "RoundOver";

		public RoundOverEntity(FrostySdk.Ebx.RoundOverEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

