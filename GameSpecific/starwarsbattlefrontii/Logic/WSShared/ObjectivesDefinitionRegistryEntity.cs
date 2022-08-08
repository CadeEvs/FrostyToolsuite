using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ObjectivesDefinitionRegistryEntityData))]
	public class ObjectivesDefinitionRegistryEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ObjectivesDefinitionRegistryEntityData>
	{
		public new FrostySdk.Ebx.ObjectivesDefinitionRegistryEntityData Data => data as FrostySdk.Ebx.ObjectivesDefinitionRegistryEntityData;
		public override string DisplayName => "ObjectivesDefinitionRegistry";

		public ObjectivesDefinitionRegistryEntity(FrostySdk.Ebx.ObjectivesDefinitionRegistryEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

