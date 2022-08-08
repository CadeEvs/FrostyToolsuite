using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ContrailsEntityData))]
	public class ContrailsEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ContrailsEntityData>
	{
		public new FrostySdk.Ebx.ContrailsEntityData Data => data as FrostySdk.Ebx.ContrailsEntityData;
		public override string DisplayName => "Contrails";

		public ContrailsEntity(FrostySdk.Ebx.ContrailsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

