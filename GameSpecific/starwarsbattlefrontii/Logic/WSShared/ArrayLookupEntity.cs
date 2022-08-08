using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ArrayLookupEntityData))]
	public class ArrayLookupEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ArrayLookupEntityData>
	{
		public new FrostySdk.Ebx.ArrayLookupEntityData Data => data as FrostySdk.Ebx.ArrayLookupEntityData;
		public override string DisplayName => "ArrayLookup";

		public ArrayLookupEntity(FrostySdk.Ebx.ArrayLookupEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

