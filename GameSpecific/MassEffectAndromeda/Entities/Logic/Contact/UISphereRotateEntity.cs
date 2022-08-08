using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UISphereRotateEntityData))]
	public class UISphereRotateEntity : UIWidgetEntity, IEntityData<FrostySdk.Ebx.UISphereRotateEntityData>
	{
		public new FrostySdk.Ebx.UISphereRotateEntityData Data => data as FrostySdk.Ebx.UISphereRotateEntityData;
		public override string DisplayName => "UISphereRotate";

		public UISphereRotateEntity(FrostySdk.Ebx.UISphereRotateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

