using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BWScaleClampData))]
	public class BWScaleClamp : LogicEntity, IEntityData<FrostySdk.Ebx.BWScaleClampData>
	{
		public new FrostySdk.Ebx.BWScaleClampData Data => data as FrostySdk.Ebx.BWScaleClampData;
		public override string DisplayName => "BWScaleClamp";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Properties
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("In", Direction.In),
				new ConnectionDesc("Out", Direction.Out)
			};
		}

        public BWScaleClamp(FrostySdk.Ebx.BWScaleClampData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

