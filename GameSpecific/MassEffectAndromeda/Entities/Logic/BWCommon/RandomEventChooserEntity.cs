using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RandomEventChooserEntityData))]
	public class RandomEventChooserEntity : LogicEntity, IEntityData<FrostySdk.Ebx.RandomEventChooserEntityData>
	{
		public new FrostySdk.Ebx.RandomEventChooserEntityData Data => data as FrostySdk.Ebx.RandomEventChooserEntityData;
		public override string DisplayName => "RandomEventChooser";

		public RandomEventChooserEntity(FrostySdk.Ebx.RandomEventChooserEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

