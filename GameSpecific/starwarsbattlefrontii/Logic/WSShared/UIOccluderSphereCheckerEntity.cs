using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIOccluderSphereCheckerEntityData))]
	public class UIOccluderSphereCheckerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UIOccluderSphereCheckerEntityData>
	{
		public new FrostySdk.Ebx.UIOccluderSphereCheckerEntityData Data => data as FrostySdk.Ebx.UIOccluderSphereCheckerEntityData;
		public override string DisplayName => "UIOccluderSphereChecker";

		public UIOccluderSphereCheckerEntity(FrostySdk.Ebx.UIOccluderSphereCheckerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

