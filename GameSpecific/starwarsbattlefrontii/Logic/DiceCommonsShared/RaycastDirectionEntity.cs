using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RaycastDirectionEntityData))]
	public class RaycastDirectionEntity : LogicEntity, IEntityData<FrostySdk.Ebx.RaycastDirectionEntityData>
	{
		public new FrostySdk.Ebx.RaycastDirectionEntityData Data => data as FrostySdk.Ebx.RaycastDirectionEntityData;
		public override string DisplayName => "RaycastDirection";

		public RaycastDirectionEntity(FrostySdk.Ebx.RaycastDirectionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

