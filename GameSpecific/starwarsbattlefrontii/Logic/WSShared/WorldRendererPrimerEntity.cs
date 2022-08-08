using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WorldRendererPrimerEntityData))]
	public class WorldRendererPrimerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WorldRendererPrimerEntityData>
	{
		public new FrostySdk.Ebx.WorldRendererPrimerEntityData Data => data as FrostySdk.Ebx.WorldRendererPrimerEntityData;
		public override string DisplayName => "WorldRendererPrimer";

		public WorldRendererPrimerEntity(FrostySdk.Ebx.WorldRendererPrimerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

