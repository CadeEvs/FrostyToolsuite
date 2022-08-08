using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIOccluderSphereEntityData))]
	public class UIOccluderSphereEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UIOccluderSphereEntityData>
	{
		public new FrostySdk.Ebx.UIOccluderSphereEntityData Data => data as FrostySdk.Ebx.UIOccluderSphereEntityData;
		public override string DisplayName => "UIOccluderSphere";

		public UIOccluderSphereEntity(FrostySdk.Ebx.UIOccluderSphereEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

