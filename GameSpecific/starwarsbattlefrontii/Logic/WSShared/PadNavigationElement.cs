using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PadNavigationElementData))]
	public class PadNavigationElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.PadNavigationElementData>
	{
		public new FrostySdk.Ebx.PadNavigationElementData Data => data as FrostySdk.Ebx.PadNavigationElementData;
		public override string DisplayName => "PadNavigationElement";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PadNavigationElement(FrostySdk.Ebx.PadNavigationElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

