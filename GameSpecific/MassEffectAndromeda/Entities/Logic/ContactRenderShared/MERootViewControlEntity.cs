using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MERootViewControlEntityData))]
	public class MERootViewControlEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MERootViewControlEntityData>
	{
		public new FrostySdk.Ebx.MERootViewControlEntityData Data => data as FrostySdk.Ebx.MERootViewControlEntityData;
		public override string DisplayName => "MERootViewControl";

		public MERootViewControlEntity(FrostySdk.Ebx.MERootViewControlEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

