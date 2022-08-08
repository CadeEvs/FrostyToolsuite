using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StopAmbientActionEntityData))]
	public class StopAmbientActionEntity : LogicEntity, IEntityData<FrostySdk.Ebx.StopAmbientActionEntityData>
	{
		public new FrostySdk.Ebx.StopAmbientActionEntityData Data => data as FrostySdk.Ebx.StopAmbientActionEntityData;
		public override string DisplayName => "StopAmbientAction";

		public StopAmbientActionEntity(FrostySdk.Ebx.StopAmbientActionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

