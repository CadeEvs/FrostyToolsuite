using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ArcElementData))]
	public class ArcElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.ArcElementData>
	{
		public new FrostySdk.Ebx.ArcElementData Data => data as FrostySdk.Ebx.ArcElementData;
		public override string DisplayName => "ArcElement";

		public ArcElement(FrostySdk.Ebx.ArcElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

