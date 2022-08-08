using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ImmediateOREntityData))]
	public class ImmediateOREntity : LogicEntity, IEntityData<FrostySdk.Ebx.ImmediateOREntityData>
	{
		public new FrostySdk.Ebx.ImmediateOREntityData Data => data as FrostySdk.Ebx.ImmediateOREntityData;
		public override string DisplayName => "ImmediateOR";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ImmediateOREntity(FrostySdk.Ebx.ImmediateOREntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

