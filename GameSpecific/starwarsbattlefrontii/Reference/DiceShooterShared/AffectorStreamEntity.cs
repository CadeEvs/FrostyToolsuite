using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AffectorStreamEntityData))]
	public class AffectorStreamEntity : LogicReferenceObject, IEntityData<FrostySdk.Ebx.AffectorStreamEntityData>
	{
		public new FrostySdk.Ebx.AffectorStreamEntityData Data => data as FrostySdk.Ebx.AffectorStreamEntityData;

		public AffectorStreamEntity(FrostySdk.Ebx.AffectorStreamEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

