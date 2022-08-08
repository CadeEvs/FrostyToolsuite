using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SphereQueryEntityData))]
	public class SphereQueryEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SphereQueryEntityData>
	{
		public new FrostySdk.Ebx.SphereQueryEntityData Data => data as FrostySdk.Ebx.SphereQueryEntityData;
		public override string DisplayName => "SphereQuery";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SphereQueryEntity(FrostySdk.Ebx.SphereQueryEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

