using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LocalPlayerLODEntityData))]
	public class LocalPlayerLODEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LocalPlayerLODEntityData>
	{
		public new FrostySdk.Ebx.LocalPlayerLODEntityData Data => data as FrostySdk.Ebx.LocalPlayerLODEntityData;
		public override string DisplayName => "LocalPlayerLOD";

		public LocalPlayerLODEntity(FrostySdk.Ebx.LocalPlayerLODEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

