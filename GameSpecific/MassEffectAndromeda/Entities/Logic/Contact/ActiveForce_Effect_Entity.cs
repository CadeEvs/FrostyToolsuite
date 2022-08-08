using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ActiveForce_Effect_EntityData))]
	public class ActiveForce_Effect_Entity : LogicEntity, IEntityData<FrostySdk.Ebx.ActiveForce_Effect_EntityData>
	{
		public new FrostySdk.Ebx.ActiveForce_Effect_EntityData Data => data as FrostySdk.Ebx.ActiveForce_Effect_EntityData;
		public override string DisplayName => "ActiveForce_Effect_";

		public ActiveForce_Effect_Entity(FrostySdk.Ebx.ActiveForce_Effect_EntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

