using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ExoticTraversalManagerEntityData))]
	public class ExoticTraversalManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ExoticTraversalManagerEntityData>
	{
		public new FrostySdk.Ebx.ExoticTraversalManagerEntityData Data => data as FrostySdk.Ebx.ExoticTraversalManagerEntityData;
		public override string DisplayName => "ExoticTraversalManager";

		public ExoticTraversalManagerEntity(FrostySdk.Ebx.ExoticTraversalManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

