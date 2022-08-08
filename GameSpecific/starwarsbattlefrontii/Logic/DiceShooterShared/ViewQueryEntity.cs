using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ViewQueryEntityData))]
	public class ViewQueryEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ViewQueryEntityData>
	{
		public new FrostySdk.Ebx.ViewQueryEntityData Data => data as FrostySdk.Ebx.ViewQueryEntityData;
		public override string DisplayName => "ViewQuery";

		public ViewQueryEntity(FrostySdk.Ebx.ViewQueryEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

