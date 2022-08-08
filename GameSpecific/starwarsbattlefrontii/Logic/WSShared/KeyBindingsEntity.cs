using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.KeyBindingsEntityData))]
	public class KeyBindingsEntity : LogicEntity, IEntityData<FrostySdk.Ebx.KeyBindingsEntityData>
	{
		public new FrostySdk.Ebx.KeyBindingsEntityData Data => data as FrostySdk.Ebx.KeyBindingsEntityData;
		public override string DisplayName => "KeyBindings";

		public KeyBindingsEntity(FrostySdk.Ebx.KeyBindingsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

