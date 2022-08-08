using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.JetPackEntityData))]
	public class JetPackEntity : LogicEntity, IEntityData<FrostySdk.Ebx.JetPackEntityData>
	{
		public new FrostySdk.Ebx.JetPackEntityData Data => data as FrostySdk.Ebx.JetPackEntityData;
		public override string DisplayName => "JetPack";

		public JetPackEntity(FrostySdk.Ebx.JetPackEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

