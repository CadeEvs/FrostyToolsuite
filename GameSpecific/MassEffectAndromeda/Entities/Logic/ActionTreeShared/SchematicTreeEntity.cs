using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SchematicTreeEntityData))]
	public class SchematicTreeEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SchematicTreeEntityData>
	{
		public new FrostySdk.Ebx.SchematicTreeEntityData Data => data as FrostySdk.Ebx.SchematicTreeEntityData;
		public override string DisplayName => "SchematicTree";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SchematicTreeEntity(FrostySdk.Ebx.SchematicTreeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

