using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEMatchModifierManagerEntityData))]
	public class MEMatchModifierManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MEMatchModifierManagerEntityData>
	{
		public new FrostySdk.Ebx.MEMatchModifierManagerEntityData Data => data as FrostySdk.Ebx.MEMatchModifierManagerEntityData;
		public override string DisplayName => "MEMatchModifierManager";
        public override IEnumerable<ConnectionDesc> Properties
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("PackageIndex", Direction.In)
			};
		}

        public MEMatchModifierManagerEntity(FrostySdk.Ebx.MEMatchModifierManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

