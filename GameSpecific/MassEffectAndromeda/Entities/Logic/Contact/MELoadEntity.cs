using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MELoadEntityData))]
	public class MELoadEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MELoadEntityData>
	{
		public new FrostySdk.Ebx.MELoadEntityData Data => data as FrostySdk.Ebx.MELoadEntityData;
		public override string DisplayName => "MELoad";
        public override IEnumerable<ConnectionDesc> Events
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Load", Direction.In)
			};
		}
        public override IEnumerable<ConnectionDesc> Properties
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("SaveName", Direction.In)
			};
        }

        public MELoadEntity(FrostySdk.Ebx.MELoadEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

