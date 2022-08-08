using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SPLoadoutStatusEntityData))]
	public class SPLoadoutStatusEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SPLoadoutStatusEntityData>
	{
		public new FrostySdk.Ebx.SPLoadoutStatusEntityData Data => data as FrostySdk.Ebx.SPLoadoutStatusEntityData;
		public override string DisplayName => "SPLoadoutStatus";

		public SPLoadoutStatusEntity(FrostySdk.Ebx.SPLoadoutStatusEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

