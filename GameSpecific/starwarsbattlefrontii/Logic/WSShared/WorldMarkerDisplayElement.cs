using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WorldMarkerDisplayElementData))]
	public class WorldMarkerDisplayElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.WorldMarkerDisplayElementData>
	{
		public new FrostySdk.Ebx.WorldMarkerDisplayElementData Data => data as FrostySdk.Ebx.WorldMarkerDisplayElementData;
		public override string DisplayName => "WorldMarkerDisplayElement";

		public WorldMarkerDisplayElement(FrostySdk.Ebx.WorldMarkerDisplayElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

