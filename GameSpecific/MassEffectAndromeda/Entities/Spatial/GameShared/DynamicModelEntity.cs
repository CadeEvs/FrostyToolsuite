using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DynamicModelEntityData))]
	public class DynamicModelEntity : DynamicGamePhysicsEntity, IEntityData<FrostySdk.Ebx.DynamicModelEntityData>
	{
		public new FrostySdk.Ebx.DynamicModelEntityData Data => data as FrostySdk.Ebx.DynamicModelEntityData;

		public DynamicModelEntity(FrostySdk.Ebx.DynamicModelEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

