using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WriteAntGameStateEntityData))]
	public class WriteAntGameStateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WriteAntGameStateEntityData>
	{
		public new FrostySdk.Ebx.WriteAntGameStateEntityData Data => data as FrostySdk.Ebx.WriteAntGameStateEntityData;
		public override string DisplayName => "WriteAntGameState";

		public WriteAntGameStateEntity(FrostySdk.Ebx.WriteAntGameStateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

