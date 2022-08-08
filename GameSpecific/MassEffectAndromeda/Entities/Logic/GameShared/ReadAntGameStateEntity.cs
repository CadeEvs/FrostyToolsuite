using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ReadAntGameStateEntityData))]
	public class ReadAntGameStateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ReadAntGameStateEntityData>
	{
		public new FrostySdk.Ebx.ReadAntGameStateEntityData Data => data as FrostySdk.Ebx.ReadAntGameStateEntityData;
		public override string DisplayName => "ReadAntGameState";

		public ReadAntGameStateEntity(FrostySdk.Ebx.ReadAntGameStateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

