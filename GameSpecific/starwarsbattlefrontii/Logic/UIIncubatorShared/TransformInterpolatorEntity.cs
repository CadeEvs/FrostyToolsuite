using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TransformInterpolatorEntityData))]
	public class TransformInterpolatorEntity : PropertyInterpolatorEntity, IEntityData<FrostySdk.Ebx.TransformInterpolatorEntityData>
	{
		public new FrostySdk.Ebx.TransformInterpolatorEntityData Data => data as FrostySdk.Ebx.TransformInterpolatorEntityData;
		public override string DisplayName => "TransformInterpolator";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public TransformInterpolatorEntity(FrostySdk.Ebx.TransformInterpolatorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

