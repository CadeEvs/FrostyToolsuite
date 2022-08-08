using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FenceModelEntityData))]
	public class FenceModelEntity : StaticModelEntity, IEntityData<FrostySdk.Ebx.FenceModelEntityData>
	{
		public new FrostySdk.Ebx.FenceModelEntityData Data => data as FrostySdk.Ebx.FenceModelEntityData;

		public FenceModelEntity(FrostySdk.Ebx.FenceModelEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

