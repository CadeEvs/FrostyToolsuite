using LevelEditorPlugin.Editors;
using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FloatProviderEntityData))]
	public class FloatProviderEntity : ProviderEntity, IEntityData<FrostySdk.Ebx.FloatProviderEntityData>
	{
		public new FrostySdk.Ebx.FloatProviderEntityData Data => data as FrostySdk.Ebx.FloatProviderEntityData;
		public override string DisplayName => "FloatProvider";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Properties
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Value", Direction.Out)
			};
		}
        public override IEnumerable<string> HeaderRows
        {
			get
            {
				List<string> outHeaderRows = new List<string>();
				if (Data.Provider.Type != FrostySdk.IO.PointerRefType.Null)
				{
					FrostySdk.Ebx.FloatProvider provider = Data.Provider.GetObjectAs<FrostySdk.Ebx.FloatProvider>();
					outHeaderRows.Add(provider.__Id);
				}
				return outHeaderRows;
            }
        }

        public FloatProviderEntity(FrostySdk.Ebx.FloatProviderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

