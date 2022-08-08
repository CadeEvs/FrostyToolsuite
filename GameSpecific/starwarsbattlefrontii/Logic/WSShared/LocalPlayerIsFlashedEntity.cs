using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LocalPlayerIsFlashedEntityData))]
	public class LocalPlayerIsFlashedEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LocalPlayerIsFlashedEntityData>
	{
		public new FrostySdk.Ebx.LocalPlayerIsFlashedEntityData Data => data as FrostySdk.Ebx.LocalPlayerIsFlashedEntityData;
		public override string DisplayName => "LocalPlayerIsFlashed";

		public LocalPlayerIsFlashedEntity(FrostySdk.Ebx.LocalPlayerIsFlashedEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

